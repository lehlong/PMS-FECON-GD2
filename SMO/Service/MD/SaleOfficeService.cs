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
using System.Data.SqlClient;
using SMO.Service.AD;
using System.Linq;

namespace SMO.Service.MD
{
    public class SaleOfficeService : GenericService<T_MD_SALEOFFICE, SaleOfficeRepo>
    {
        public SaleOfficeService() : base()
        {

        }

        public void UpdateDischard(string code, string value, string field)
        {
            try
            {
                this.Get(code);
                if (field == "dischard")
                {
                    this.ObjDetail.DISCHARD_POINT = value;
                }
                if (field == "email")
                {
                    this.ObjDetail.EMAIL = value;
                }
                if (field == "phone")
                {
                    this.ObjDetail.PHONE = value;
                }
                base.Update();
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
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
                    var result = conn.ExecuteFunction(new SynMD_SaleOffice_Function() { Parameters = new { I_CHANEL = "08", I_SALEORG  = "6630"} }).ToList();

                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_MD_SALEOFFICE";
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
                        var row = dataTable.Rows.Find(item.CODE);
                        if (row == null)
                        {
                            DataRow newRow = dataTable.NewRow();
                            newRow["CODE"] = item.CODE;
                            newRow["TEXT"] = item.TEXT;
                            newRow["COMPANY_CODE"] = item.COMPANY_CODE;
                            newRow["ACTIVE"] = "Y";
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                        else if (row["TEXT"].ToString() != item.TEXT)
                        {
                            row.BeginEdit();
                            row["TEXT"] = item.TEXT;
                            row["UPDATE_BY"] = actionBy;
                            row["UPDATE_DATE"] = actionDate;
                            row.EndEdit();
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
