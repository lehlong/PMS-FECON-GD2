using NHibernate;
using SharpSapRfc;
using SharpSapRfc.Plain;
using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using SMO.Service.AD;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SMO.Service.MD
{
    public class DischardService : GenericService<T_MD_DISCHARD, DischardRepo>
    {
        public DischardService() : base()
        {

        }

        public void GetDischardPointOfCustomer(string customerCode)
        {
            try
            {
                customerCode = customerCode.TrimStart('0');
                this.ObjList = UnitOfWork.GetSession().Query<T_MD_DISCHARD>().Where(x => x.CUSTOMER_CODE == customerCode).ToList();
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public void UpdateDischard(string value, string code, string type)
        {
            try
            {
                this.Get(code);
                if (type == "0")
                {
                    this.ObjDetail.TEXT = value;
                }
                else
                {
                    this.ObjDetail.CUSTOMER_CODE = value;
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
                    var result = conn.ExecuteFunction(new SynMD_Dischard_Function()).ToList();

                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_MD_DISCHARD";
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
                            newRow["CUSTOMER_CODE"] = item.CODE.Length > 6 ? item.CODE.Substring(0,6) : string.Empty;
                            newRow["ACTIVE"] = "N";
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
