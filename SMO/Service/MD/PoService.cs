using NHibernate;
using SharpSapRfc;
using SharpSapRfc.Plain;
using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using System;
using System.Configuration;
using System.Data;
using SMO.Service.AD;
using System.Data.SqlClient;
using System.Linq;

namespace SMO.Service.MD
{
    public class PoService : GenericService<T_MD_PO, PoRepo>
    {
        public PoService() : base()
        {

        }

        public void Synchronize(string companyCode, string poType)
        {
            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                    systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var result = conn.ExecuteFunction(new SynMD_PO_Function() {
                        Parameters = new {
                            I_COMPANYCODE = companyCode,
                            I_DATE = new DateTime(2018,01,01),
                            I_POTYPE = poType
                        }
                    }).ToList();

                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_MD_PO";
                    SqlDataAdapter daAdapter = new SqlDataAdapter("SELECT * FROM " + tableName, objConn);
                    daAdapter.UpdateBatchSize = 0;
                    DataSet dataSet = new DataSet(tableName);
                    daAdapter.FillSchema(dataSet, SchemaType.Source, tableName);
                    daAdapter.Fill(dataSet, tableName);
                    DataTable dataTable;
                    dataTable = dataSet.Tables[tableName];

                    var actionBy = ProfileUtilities.User.USER_NAME;
                    var actionDate = DateTime.Now;
                    foreach (var item in result)
                    {
                        var where = string.Format("PO_NUMBER = '{0}' AND PO_ITEM = {1}", item.PO_NUMBER, item.PO_ITEM);
                        var row = dataTable.Select(where);
                        if (row == null || row.Count() == 0)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["PKID"] = Guid.NewGuid().ToString();
                            newRow["PO_NUMBER"] = item.PO_NUMBER;
                            newRow["PO_ITEM"] = item.PO_ITEM;
                            newRow["PO_TYPE"] = item.PO_TYPE;
                            newRow["PO_ORG"] = item.PO_ORG;
                            newRow["PO_GROUP"] = item.PO_GROUP;
                            newRow["PO_DATE"] = item.PO_DATE;
                            newRow["MATERIAL_CODE"] = item.MATERIAL_CODE;
                            newRow["PLANT_CODE"] = item.PLANT_CODE;
                            newRow["QUANITY"] = item.QUANITY;
                            newRow["UNIT_CODE"] = item.UNIT_CODE;
                            newRow["PO_LOCK"] = item.PO_LOCK;
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                    }

                    SqlCommandBuilder objCommandBuilder = new SqlCommandBuilder(daAdapter);
                    daAdapter.Update(dataSet, tableName);
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
