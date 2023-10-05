using NHibernate;
using SharpSapRfc;
using SharpSapRfc.Plain;
using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using SMO.Service.AD;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
#pragma warning disable CS0105 // The using directive for 'SMO.Service.AD' appeared previously in this namespace
using SMO.Service.AD;
#pragma warning restore CS0105 // The using directive for 'SMO.Service.AD' appeared previously in this namespace
using System.Data.SqlClient;
using System.Linq;

namespace SMO.Service.MD
{
    public class VehicleService : GenericService<T_MD_VEHICLE, VehicleRepo>
    {
        public VehicleService() : base()
        {

        }

        public void Synchronize()
        {
            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                    systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new SynMD_Vehicle_Function();
                    if (systemConfig.ObjDetail.LAST_SYN_VEHICLE.HasValue)
                    {
                        functionSAP.Parameters = new
                        {
                            I_DATE = systemConfig.ObjDetail.LAST_SYN_VEHICLE.Value.AddDays(-30)
                        };
                    }
                    
                    var result = conn.ExecuteFunction(functionSAP).ToList();
                    if (result.Count == 0)
                    {
                        return;
                    }
                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_MD_VEHICLE";
                    string tableNameComparment = "T_MD_VEHICLE_COMPARTMENT";
                    SqlDataAdapter daAdapter = new SqlDataAdapter("SELECT * FROM " + tableName, objConn);
                    SqlDataAdapter daAdapterComparment = new SqlDataAdapter("SELECT * FROM " + tableNameComparment, objConn);
                    daAdapter.UpdateBatchSize = 0;
                    daAdapterComparment.UpdateBatchSize = 0;
                    DataSet dataSet = new DataSet(tableName);
                    DataSet dataSetComparment = new DataSet(tableNameComparment);
                    daAdapter.FillSchema(dataSet, SchemaType.Source, tableName);
                    daAdapter.Fill(dataSet, tableName);
                    daAdapterComparment.FillSchema(dataSetComparment, SchemaType.Source, tableNameComparment);
                    daAdapterComparment.Fill(dataSetComparment, tableNameComparment);
                    DataTable dataTable;
                    DataTable dataTableComparment;
                    dataTable = dataSet.Tables[tableName];
                    dataTableComparment = dataSetComparment.Tables[tableNameComparment];

                    var actionBy = "tichhop";
                    var actionDate = DateTime.Now;


                    var lstVehicle = from x in result
                                     group x by new { x.VEHICLE_CODE, x.TRANSUNIT_CODE, x.UNIT, x.TRANSMODE_CODE, x.OIC_PBATCH, x.OIC_PTRIP } into grp
                                     select new
                                     {
                                         CODE = grp.Key.VEHICLE_CODE,
                                         TRANSUNIT_CODE = grp.Key.TRANSUNIT_CODE,
                                         TRANSMODE_CODE = grp.Key.TRANSMODE_CODE,
                                         UNIT = grp.Key.UNIT,
                                         OIC_PBATCH = grp.Key.OIC_PBATCH,
                                         OIC_PTRIP = grp.Key.OIC_PTRIP,
                                         CAPACITY = grp.Sum(x => x.CAPACITY)
                                     }
                                     ;
                    lstVehicle = lstVehicle.Distinct();

                    var lstCompartment = result.Select(x => new T_MD_VEHICLE_COMPARTMENT
                    {
                        CODE = x.VEHICLE_CODE + x.SEQ_NUMBER,
                        VEHICLE_CODE = x.VEHICLE_CODE,
                        SEQ_NUMBER = x.SEQ_NUMBER,
                        CAPACITY = x.CAPACITY
                    });

                    foreach (var item in lstVehicle)
                    {
                        var row = dataTable.Rows.Find(item.CODE);
                        if (row == null)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["CODE"] = item.CODE;
                            newRow["TRANSUNIT_CODE"] = item.TRANSUNIT_CODE;
                            newRow["TRANSMODE_CODE"] = item.TRANSMODE_CODE;
                            newRow["OIC_PBATCH"] = item.OIC_PBATCH;
                            newRow["OIC_PTRIP"] = item.OIC_PTRIP;
                            newRow["UNIT"] = item.UNIT;
                            newRow["CAPACITY"] = item.CAPACITY;
                            newRow["ACTIVE"] = "N";
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                        else if (row["TRANSUNIT_CODE"].ToString() != item.TRANSUNIT_CODE ||
                            row["TRANSMODE_CODE"].ToString() != item.TRANSMODE_CODE ||
                            row["UNIT"].ToString() != item.UNIT ||
                            row["CAPACITY"].ToString() != item.CAPACITY.ToString()
                            )
                        {
                            row.BeginEdit();
                            row["TRANSUNIT_CODE"] = item.TRANSUNIT_CODE;
                            row["TRANSMODE_CODE"] = item.TRANSMODE_CODE;
                            row["UNIT"] = item.UNIT;
                            row["CAPACITY"] = item.CAPACITY;
                            row["UPDATE_BY"] = actionBy;
                            row["UPDATE_DATE"] = actionDate;
                            row.EndEdit();
                        }
                    }

                    SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daAdapter);
                    daAdapter.Update(dataSet, tableName);

                    foreach (var item in lstCompartment)
                    {
                        var row = dataTableComparment.Rows.Find(new object[] { item.CODE, item.VEHICLE_CODE});
                        if (row == null)
                        {
                            DataRow newRow = dataTableComparment.NewRow();
                            newRow["CODE"] = item.CODE;
                            newRow["VEHICLE_CODE"] = item.VEHICLE_CODE;
                            newRow["SEQ_NUMBER"] = item.SEQ_NUMBER;
                            newRow["CAPACITY"] = item.CAPACITY;
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTableComparment.Rows.Add(newRow);
                        }
                        else if (row["SEQ_NUMBER"].ToString() != item.SEQ_NUMBER || row["CAPACITY"].ToString() != item.CAPACITY.ToString())
                        {
                            row.BeginEdit();
                            row["SEQ_NUMBER"] = item.SEQ_NUMBER;
                            row["CAPACITY"] = item.CAPACITY;
                            row["UPDATE_BY"] = actionBy;
                            row["UPDATE_DATE"] = actionDate;
                            row.EndEdit();
                        }
                    }

                    SqlCommandBuilder objCommandBuilderComparment = new SqlCommandBuilder(daAdapterComparment);
                    daAdapterComparment.Update(dataSetComparment, tableNameComparment);

                    systemConfig.ObjDetail.LAST_SYN_VEHICLE = DateTime.Now;
                    systemConfig.Update();
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
