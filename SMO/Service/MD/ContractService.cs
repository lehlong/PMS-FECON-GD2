using NHibernate;
using SharpSapRfc;
using SharpSapRfc.Plain;
using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using System;
using System.Configuration;
using SMO.Service.AD;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace SMO.Service.MD
{
    public class ContractService : GenericService<T_MD_CONTRACT, ContractRepo>
    {
        public ContractService() : base()
        {

        }

        public void Synchronize(string companyCode)
        {
            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                    systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var result = conn.ExecuteFunction(new SynMD_Contract_Function() {
                        Parameters = new { I_SALEORG = companyCode }
                    }).ToList();

                    SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString);
                    objConn.Open();

                    string tableName = "T_MD_CONTRACT";
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
                            newRow["CONTRACT_TYPE"] = item.CONTRACT_TYPE;
                            newRow["SALEORG"] = item.SALEORG;
                            newRow["DC_CODE"] = item.DC_CODE;
                            newRow["DIVISION_CODE"] = item.DIVISION_CODE;
                            newRow["CUSTOMER_CODE"] = item.CUSTOMER_CODE;
                            newRow["INCOTERMS1"] = item.INCOTERMS1;
                            newRow["INCOTERMS2"] = item.INCOTERMS2;
                            newRow["PAYMENTTERM_CODE"] = item.PAYMENTTERM_CODE;
                            newRow["VALID_FROM"] = item.VALID_FROM;
                            newRow["VALID_TO"] = item.VALID_TO;
                            newRow["CREATE_BY"] = actionBy;
                            newRow["CREATE_DATE"] = actionDate;
                            dataTable.Rows.Add(newRow);
                        }
                        else if (
                            row["CONTRACT_TYPE"].ToString() != item.CONTRACT_TYPE ||
                            row["SALEORG"].ToString() != item.SALEORG ||
                            row["DC_CODE"].ToString() != item.DC_CODE ||
                            row["DIVISION_CODE"].ToString() != item.DIVISION_CODE ||
                            row["CUSTOMER_CODE"].ToString() != item.CUSTOMER_CODE ||
                            row["INCOTERMS1"].ToString() != item.INCOTERMS1 ||
                            row["INCOTERMS2"].ToString() != item.INCOTERMS2 ||
                            row["VALID_FROM"].ToString() != item.VALID_FROM.Value.ToString("MM/dd/YYYY HH:mm:ss") ||
                            row["VALID_TO"].ToString() != item.VALID_TO.Value.ToString("MM/dd/YYYY HH:mm:ss")
                            )
                        {
                            row.BeginEdit();
                            row["CONTRACT_TYPE"] = item.CONTRACT_TYPE;
                            row["SALEORG"] = item.SALEORG;
                            row["DC_CODE"] = item.DC_CODE;
                            row["DIVISION_CODE"] = item.DIVISION_CODE;
                            row["CUSTOMER_CODE"] = item.CUSTOMER_CODE;
                            row["INCOTERMS1"] = item.INCOTERMS1;
                            row["INCOTERMS2"] = item.INCOTERMS2;
                            row["PAYMENTTERM_CODE"] = item.PAYMENTTERM_CODE;
                            row["VALID_FROM"] = item.VALID_FROM;
                            row["VALID_TO"] = item.VALID_TO;
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
