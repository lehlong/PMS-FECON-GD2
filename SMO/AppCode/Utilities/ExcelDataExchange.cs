using ExcelDataReader;

using System.Data;
using System.IO;

namespace SMO.AppCode.Utilities
{
    public class ExcelDataExchange
    {
        public static DataTable ReadData(string filePath)
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                try
                {
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    DataSet dataset = excelReader.AsDataSet();
                    if (dataset != null && dataset.Tables.Count > 0)
                    {
                        return dataset.Tables[0];
                    }
                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
