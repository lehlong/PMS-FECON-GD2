using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using System.Web;
using System.Web.Routing;
using System.Xml;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Globalization;

namespace SMO
{
    public static class UtilsCore
    {
        public static string ConvertCoDauKhongDau(string s)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = s.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D');
        }
        public static string[] GetValueOfObject<T>(object pObjData, string pStrColumn)
        {
            if (pObjData != null)
            {
                List<string> vArrValue = new List<string>();
                var items = (IEnumerable<T>)pObjData;
                foreach (var item in items)
                {
                    var value = typeof(T).GetProperty(pStrColumn).GetValue(item, null) ?? String.Empty;
                    var type = typeof(T).GetProperty(pStrColumn).PropertyType;
                    if (type.Name == "Boolean" || type.Name == "bool")
                    {
                        vArrValue.Add(value.ToString().ToUpper() == "TRUE" ? "1" : "0");
                    }
                    else
                    {
                        vArrValue.Add(value.ToString());
                    }
                }
                return vArrValue.ToArray();
            }
            else
            {
                return new string[1] {""};
            }            
        }

        public static void WriteFile(string data, string filePath)
        {
            try
            {
                StreamWriter sWriter;

                if (!File.Exists(filePath))
                {
                    sWriter = new StreamWriter(filePath,false, Encoding.ASCII);
                }
                else
                {
                    sWriter = File.AppendText(filePath);
                }

                // Write to the file:
                sWriter.Write(data);                
                sWriter.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void WriteFile(string data, string filePath, bool unicode)
        {
            try
            {
                StreamWriter sWriter;

                if (!File.Exists(filePath))
                {
                    if (unicode)
                        sWriter = new StreamWriter(filePath, false, Encoding.UTF8);
                    else
                        sWriter = new StreamWriter(filePath, false, Encoding.ASCII);
                }
                else
                {
                    sWriter = File.AppendText(filePath);
                }

                // Write to the file:
                sWriter.Write(data);
                sWriter.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CopyFile(string sourceDirectory, string destDirectory)
        {
            DirectoryInfo source = new DirectoryInfo(sourceDirectory);
            DirectoryInfo target = new DirectoryInfo(destDirectory);
            // If the target doesn't exist, we create it.
            if (!target.Exists)
                target.Create();

            // Get all files and copy them over.
            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
            }

        }

        public static void OverWriteFile(string data, string filePath)
        {
            try
            {
                StreamWriter sWriter;

                sWriter = new StreamWriter(filePath);
                // Write to the file:
                sWriter.Write(data);
                sWriter.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string[] ReadFileText(string pFileName)
        {
            List<string> allFile = new List<string>();
            try
            {
                using (FileStream fileStream = new FileStream(pFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 8, FileOptions.Asynchronous))
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(fileStream))
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            allFile.Add(line);
                        }
                    }
                }
                return allFile.ToArray();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ReadAllFileText(string pStrFileName)
        {
            try
            {
                FileInfo file = new FileInfo(pStrFileName);
                string vContent = file.OpenText().ReadToEnd();
                return vContent;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataSet ReadFile(string pFileName)
        {
            DataSet newDataSet = new DataSet();
            try
            {
                if (File.Exists(pFileName))
                {
                    newDataSet.ReadXml(pFileName, XmlReadMode.Auto);
                }                
                return newDataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                newDataSet.Dispose();
            }
        }

        public static bool DeleteFile(string fileToDelete)
        {

            try
            {
                if (File.Exists(fileToDelete))
                {
                    File.Delete(fileToDelete);
                    return true;
                }
                else
                    return true;

            }
            catch (Exception)
            {                
                return false;
            }
        }

        public static bool CreateDirctory(string pStrPath)
        {
            try
            {
                if (!System.IO.Directory.Exists(pStrPath))
                {
                    System.IO.Directory.CreateDirectory(pStrPath);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool DirctoryExists(string pStrPath)
        {
            try
            {
                return System.IO.Directory.Exists(pStrPath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool FileExists(string pStrPath)
        {
            try
            {
                return System.IO.File.Exists(pStrPath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        ///--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Them chuoi so "0" vao truoc 
        /// </summary>
        /// <param name="pStrValue">Chuoi can them</param>
        /// <param name="pIntLen">Do dai can thiet</param>
        /// <returns>Chuoi co do dai = pIntLen</returns>
        public static string Padding(string pStrValue, int pIntLen)
        {
            string strReturnValue = pStrValue;
            int len = pStrValue.Length;

            // Kiem tra do dai de cong them "0" vao truoc
            if ((pIntLen > len) && (len >= 1))
            {
                for (int i = 0; i < pIntLen - len; i++)
                {
                    strReturnValue = "0" + strReturnValue;
                }
            }
            return strReturnValue;
        }
        ///--------------------------------------------------------------------------------------------------
        /// <summary>
        /// Bo ky tu da biet
        /// </summary>
        /// <param name="pStrValue">Chuoi can loai</param>
        /// <param name="pStrSepecialChar">Chuoi cac ky tu dac biet can loai bo</param>
        /// <returns>Chuoi da duoc loai bo cac ky tu dac biet</returns>
        public static string ReplaceSepecialChar(string pStrValue, string pStrSepecialChar)
        {
            string strReturnValue = pStrValue;
            string strSepecialString = "";
            if (pStrSepecialChar != "")
            {
                strSepecialString = pStrSepecialChar;
            }
            else
            {
                strSepecialString = "~,`,!,@,#,$,%,^,&,*,\\,/,_,-";
            }
            string[] arrChar = strSepecialString.Split(',');
            int intLen = arrChar.Length;
            for (int i = 0; i < intLen; i++)
            {
                strReturnValue = strReturnValue.Replace(arrChar[i], " ");
            }
            return strReturnValue;
        }
        
        
        /// <summary>
        /// Check if file is being used
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CheckIfFileIsBeingUsed(string fileName)
        {
            try
            {
                System.IO.FileStream fs;
                fs = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                fs.Close();
            }
            catch
            {
                return true;
            }
            return false;

        }
        
        /// <summary>
        /// Lay gia tri cua mot tham so
        /// </summary>
        /// <param name="pParam"></param>
        /// <returns></returns>
        public static string GetParam(string pParam)
        {

            string strListId = "";
            for (int i = 0; i < HttpContext.Current.Request.Params.Count; i++)
            {
                if (ListHaveElement(HttpContext.Current.Request.Params.Keys[i], pParam, "$"))
                {
                    strListId = HttpContext.Current.Request.Params[i];
                    return strListId;
                }
            }
            return "";
        }

        

        /// <summary>
        /// Kiem tra mot phan tu co ton tai trong danh sach
        /// </summary>
        /// <param name="pStrList"></param>
        /// <param name="pStrElement"></param>
        /// <param name="pStrDelimitor"></param>
        /// <returns></returns>
        public static bool ListHaveElement(string pStrList, string pStrElement, string pStrDelimitor)
        {
            string vStrList = UtilsCore.ObjToString(pStrList);
            string[] stringSeparators = new string[] { pStrDelimitor };
            string[] arrList = vStrList.Split(stringSeparators, StringSplitOptions.None);

            for (int i = 0; i < arrList.Length; i++)
            {
                if (string.Compare(pStrElement, arrList[i], true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Them mot phan tu vao danh sach
        /// </summary>
        /// <param name="the_list"></param>
        /// <param name="the_value"></param>
        /// <param name="the_separator"></param>
        /// <returns></returns>
        public static string ListAppend(string the_list, string the_value, string the_separator)
        {
            string list = the_list;
            if (list.Trim() == "")
            {
                list = the_value;
            }
            else
                if (the_value.Trim() != "")
                {
                    list = list + the_separator + the_value;
                }
            return list;
        }

        /// <summary>
        /// Lay vitual directory
        /// </summary>
        /// <returns></returns>
        public static string GetVirtualPath()
        {
            return HttpRuntime.AppDomainAppVirtualPath;
        }

        /// <summary>
        /// Tra lai mot xau lap di lap lai count lan
        /// </summary>
        /// <param name="pStrInput"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static string StrRepeat(string pStrInput, int count)
        {
            string strTemp = "";
            for (int i = 0; i < count; i++)
                strTemp += pStrInput;
            return strTemp;
        }

        
        /// <summary>
        /// Chuyen xau sang so Int
        /// </summary>
        /// <param name="pStrString"></param>
        /// <returns>int</returns>
        public static int StringToInt(string pStrString)
        {
            int intValue;
            try
            {
                intValue = int.Parse(pStrString);
            }
            catch (FormatException)
            {
                intValue = 0;
            }
            catch (OverflowException)
            {
                intValue = 0;
            }
            catch (Exception)
            {
                intValue = 0;
            }
            return intValue;
        }
        /// <summary>
        /// Chuyen xau sang so Long
        /// </summary>
        /// <param name="pStrString"></param>
        /// <returns>int</returns>
        public static long StringToLong(string pStrString)
        {
            long longValue = 0;
            try
            {
                longValue = long.Parse(pStrString);
            }
            catch (Exception)
            {
                longValue = 0;
            }
            return longValue;
        }
        /// <summary>
        /// Chuyen xau sang so Double
        /// </summary>
        /// <param name="pStrString"></param>
        /// <returns>int</returns>
        public static double StringToDouble(string pStrString)
        {
            double doubletValue = 0;
            try
            {
                doubletValue = double.Parse(pStrString);
            }
            catch (Exception)
            {
                doubletValue = 0;
            }
            return doubletValue;
        }

        /// <summary>
        /// Chuyen xau sang so Double
        /// </summary>
        /// <param name="pStrString"></param>
        /// <returns>int</returns>
        public static decimal StringToDecimal(string pStrString)
        {
            decimal value = 0;
            try
            {
                value = decimal.Parse(pStrString);
            }
            catch (Exception)
            {
                value = 0;
            }
            return value;
        }
        /// <summary>
        /// Chuyen xau sang so Float
        /// </summary>
        /// <param name="pStrString"></param>
        /// <returns>int</returns>
        public static float StringToFloat(string pStrString)
        {
            float fltValue;
            try
            {
                fltValue = float.Parse(pStrString);
            }
            catch (FormatException)
            {
                fltValue = 0;
            }
            catch (OverflowException)
            {
                fltValue = 0;
            }
            catch (Exception)
            {
                fltValue = 0;
            }
            return fltValue;
        }


        /// <summary>
        /// Chuyen DateTime sang xau
        /// </summary>
        /// <param name="pStrString"></param>
        /// <returns>int</returns>    
        public static string DateTimeToString(string pStrString)
        {
            string vTemp = "";
            try
            {
                vTemp = System.Convert.ToDateTime(pStrString).ToString("dd/mm/yyyy");
            }
            catch (Exception)
            {
                vTemp = "";
            }
            return vTemp;
        }

        /// <summary>
        /// Chuyen DateTime sang xau dd/MM/yyyy
        /// </summary>
        /// <param name="pStrString"></param>
        /// <returns>int</returns>    
        public static string DateTimeToDDMMYYYY(string pStrString)
        {
            string vTemp = "";
            try
            {
                vTemp = Convert.ToDateTime(pStrString).ToString("dd/MM/yyyy");
            }
            catch (Exception)
            {
                vTemp = "";
            }
            return vTemp;
        }




        /// <summary>
        /// Thay cho ham ToString() mac dinh
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns>int</returns>
        public static string ObjToString(object pObj)
        {
            string strResult = "";
            try
            {
                strResult = pObj.ToString();
            }
            catch (Exception)
            {
                strResult = "";
            }
            return strResult;
        }

        public static byte[] GetFileContent(string pStrFilePath)
        {

            FileStream fs = new FileStream(pStrFilePath, FileMode.Open, FileAccess.Read);


            byte[] b = new byte[fs.Length - 1];


            fs.Read(b, 0, b.Length);


            fs.Close();

            return b;
        }
        /// <summary>
        /// So sanh ngay, ngay dinh dang: dd/mm/yyyy va ngay phai hop le
        /// </summary>
        /// <param name="pStrDate1">Tu ngay</param>
        /// <param name="pStrDate2">Den ngay</param>
        /// <returns>pStrDate1 - pStrDate2</returns>
        public static int CompareDate(string pStrDate1, string pStrDate2)
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("vi-VN", true);
            return DateTime.Compare(DateTime.Parse(pStrDate1, format, System.Globalization.DateTimeStyles.AllowWhiteSpaces), DateTime.Parse(pStrDate2, format, System.Globalization.DateTimeStyles.AllowWhiteSpaces));
        }
        public static int DayDiff(string pStrDate1, string pStrDate2)
        {
            System.IFormatProvider format = new System.Globalization.CultureInfo("vi-VN", true);
            TimeSpan ts = DateTime.Parse(pStrDate1, format, System.Globalization.DateTimeStyles.AllowWhiteSpaces).Subtract(DateTime.Parse(pStrDate2, format, System.Globalization.DateTimeStyles.AllowWhiteSpaces));
            return ts.Days;
        }

        public static bool IsDate(string strDate)
        {
            bool blnResult = false;
            try
            {
                DateTime.Parse(strDate);
                blnResult = true;
            }
            catch
            {
                blnResult = false;
            }
            return blnResult;
        }
        ///-----------------------------------------------------------------------------------
        /// <summary>
        /// Ham doc mot chuoi co doi dai toi da la 3
        /// </summary>
        /// <param name="pStrKey"></param>
        /// <returns></returns>
        public static string Len3ToString(string pStrInt)
        {
            string strNumber = "";
            int len = pStrInt.Length;
            char[] arr = new char[5];
            if ((pStrInt == "000") || (pStrInt == ""))
            {
                return strNumber;
            }
            else
            {
                arr = pStrInt.ToCharArray();
                switch (arr[0])
                {
                    case '0':
                        if (len == 3 || len == 1)
                        {
                            strNumber += " không";
                        }
                        break;

                    case '1':
                        if (len == 2)
                        {
                            strNumber += " mười";
                        }
                        else
                        {
                            strNumber += " một";
                        }
                        break;
                    case '2':
                        strNumber += " hai";
                        break;
                    case '3':
                        strNumber += " ba";
                        break;
                    case '4':
                        strNumber += " bốn";
                        break;
                    case '5':
                        strNumber += " năm";
                        break;
                    case '6':
                        strNumber += " sáu";
                        break;
                    case '7':
                        strNumber += " bẩy";
                        break;
                    case '8':
                        strNumber += " tám";
                        break;
                    case '9':
                        strNumber += " chín";
                        break;
                }
                if (len == 3)
                {
                    // hàng Trăm
                    strNumber += " trăm";
                }
                if (len == 2)
                {
                    if ((arr[0] != '0') && (arr[0] != '1'))
                    {
                        strNumber += " mươi";
                    }
                }
                // Hàng Chục
                if (len >= 2)
                {
                    switch (arr[1])
                    {
                        case '1':
                            if (len == 3)
                            {
                                strNumber += " mười";
                            }
                            if ((len == 2) && (arr[0] != '1'))
                            {
                                strNumber += " mốt";
                            }
                            if ((len == 2) && (arr[0] == '1'))
                            {
                                strNumber += " một";
                            }
                            break;
                        case '2':
                            strNumber += " hai";
                            break;
                        case '3':
                            strNumber += " ba";
                            break;
                        case '4':
                            //if (len == 2)
                            //{
                            //    strNumber += " tư";
                            //}
                            //else
                            //{
                            strNumber += " bốn";
                            //}
                            break;
                        case '5':
                            if (len == 2)
                            {
                                strNumber += " lăm";
                            }
                            else
                            {
                                strNumber += " năm";
                            }
                            break;
                        case '6':
                            strNumber += " sáu";
                            break;
                        case '7':
                            strNumber += " bẩy";
                            break;
                        case '8':
                            strNumber += " tám";
                            break;
                        case '9':
                            strNumber += " chín";
                            break;
                    }
                    if (len == 3)
                    {
                        if ((arr[1] != '0') && (arr[1] != '1'))
                        {
                            strNumber += " mươi";
                        }
                        if ((arr[1] == '0') && (arr[2] != '0'))
                        {
                            strNumber += " linh";
                        }
                    }
                }
                // Hàng đơn vị
                if (len == 3)
                {
                    switch (arr[2])
                    {
                        case '1':
                            if (arr[1] != '0')
                            {
                                strNumber += " mốt";
                            }
                            else
                            {
                                strNumber += " một";
                            }
                            break;
                        case '2':
                            strNumber += " hai";
                            break;
                        case '3':
                            strNumber += " ba";
                            break;
                        case '4':
                            //if ((arr[1] != '0') && (arr[1] != '1'))
                            //{
                            //    strNumber += " tư";
                            //}
                            //else
                            //{
                            strNumber += " bốn";
                            //}
                            break;
                        case '5':
                            if (arr[1] != '0')
                            {
                                strNumber += " lăm";
                            }
                            else
                            {
                                strNumber += " năm";
                            }
                            break;
                        case '6':
                            strNumber += " sáu";
                            break;
                        case '7':
                            strNumber += " bẩy";
                            break;
                        case '8':
                            strNumber += " tám";
                            break;
                        case '9':
                            strNumber += " chín";
                            break;
                    }
                }
                return strNumber;
            }
        }
        public static bool IsDate(string value, string format)
        {
            if (format.ToLower() == "dd/mm/yyyy")
            {
                string strRegex = @"^(?=\d)(?:(?:31(?!.(?:0?[2469]|11))|(?:30|29)(?!.0?2)|29(?=.0?2.(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00)))(?:\x20|$))|(?:2[0-8]|1\d|0?[1-9]))([-./])(?:1[012]|0?[1-9])\1(?:1[6-9]|[2-9]\d)?\d\d(?:(?=\x20\d)\x20|$))?(((0?[1-9]|1[012])(:[0-5]\d){0,2}(\x20[AP]M))|([01]\d|2[0-3])(:[0-5]\d){1,2})?$";
                Regex re = new Regex(strRegex);
                if (re.IsMatch(value))
                    return (true);
                else
                    return (false);
            }
            else
            {
                return (true);
            }
        }
        ///-----------------------------------------------------------------------------------
        /// <summary>
        /// Ham doc mot chuoi co doi dai bat ky
        /// </summary>
        /// <param name="pStrNumeric">Chuoi so da duoc</param>
        /// <returns>Chuỗi đọc số</returns>
        public static string NumericIntToString(string pStrNumeric)
        {
            string strNumber = "";
            string strReadNumber = "";
            string strRead3 = "";
            char[] arr = new char[100];
            int i = 0;
            int count = 0;
            int len = 0;
            int len3 = 0;
            int mod3 = 0;
            // Loại bỏ dấu phân cách nhóm
            strNumber = pStrNumeric.Replace(",", "");
            strNumber = strNumber.Replace(".", "");

            // Loại bỏ số 0 đứng đầu
            arr = strNumber.ToCharArray();
            for (i = 0; i < arr.Length; i++)
            {
                if (arr[i] == 0)
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            strNumber = strNumber.Substring(count);

            // Doc chuoi so
            len = strNumber.Length;
            // Dem bo so 3
            len3 = len / 3;
            mod3 = len % 3;

            // Neu doi dai chuoi la 0 thi gan la so 0
            if (len == 0)
            {
                strNumber = "0";
                mod3 = 1;
            }
            // Doc bo so dau tien
            strRead3 = Len3ToString(strNumber.Substring(0, mod3));
            strReadNumber = strRead3;
            for (i = 0; i < len3; i++)
            {
                if (((3 * (len3 - i)) % 9 == 0) && (strReadNumber.Length > 0))
                {
                    strReadNumber += " tỷ";
                }
                if (strRead3.Length > 0)
                {
                    if ((3 * (len3 - i)) % 9 == 6)
                    {
                        strReadNumber += " triệu";
                    }
                    if (((3 * (len3 - i)) % 9 == 3))
                    {
                        strReadNumber += " nghìn";
                    }
                }
                // Đọc chuỗi 3 kí tự
                strRead3 = Len3ToString(strNumber.Substring(mod3 + i * 3, 3));
                // Luu chuỗi còn lại
                //strNumber = strNumber.Substring(mod3 + i * 3);
                // Gán vào chuỗi kết quả đọc
                strReadNumber += strRead3;
            }

            return strReadNumber.Substring(1, 1).ToUpper() + strReadNumber.Substring(2);
        }


        /// <summary>
        /// Lay gia tri cua mot khoa
        /// </summary>
        /// <param name="pStrKey"></param>
        /// <returns></returns>
        public static string GetParameterValue(string pStrKey)
        {
            string strAppCode = ConfigurationManager.AppSettings.Get(pStrKey);
            return (string)strAppCode;
        }

        /// <summary>
        /// Convert gia tri nullable to String
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns></returns>
        public static string NumberToString(object pObj)
        {
            return pObj == null ? "" : pObj.ToString();
        }

        public static List<T> ConvertDataToListExtends<T>(DataTable data) where T : new()
        {
            List<T> converter = new List<T>();

            if (data != null)
            {
                foreach (DataRow item in data.Rows)
                {
                    T obj = Activator.CreateInstance<T>();
                    IList<PropertyInfo> propertyInfos = typeof(T).GetProperties();

                    foreach (PropertyInfo propertyInfo in propertyInfos)
                    {

                        string columnName = propertyInfo.Name;
                        if (columnName != null)
                        {
                            if (!data.Columns.Contains(columnName))
                            {
                                continue;
                            }
                            string columnValue = item[columnName] != DBNull.Value ? item[columnName].ToString() : String.Empty;

                            try
                            {
                                // propertyType is Int32
                                if ((propertyInfo.PropertyType == typeof(Int32)) || (propertyInfo.PropertyType == typeof(Int32?)))
                                {
                                    int val = 0;
                                    if (int.TryParse(columnValue, out val))
                                    {
                                        propertyInfo.SetValue(obj, val, null);
                                    }
                                }
                                else if ((propertyInfo.PropertyType == typeof(DateTime)) || (propertyInfo.PropertyType == typeof(DateTime?)))
                                {
                                    // propertyType is DateTime
                                    DateTime dt;
                                    if (DateTime.TryParse(columnValue, out dt))
                                    {
                                        propertyInfo.SetValue(obj, dt, null);
                                    }

                                }
                                else if ((propertyInfo.PropertyType == typeof(Decimal)) || (propertyInfo.PropertyType == typeof(Decimal?)))
                                {
                                    // propertyType is Decimal
                                    Decimal dc;
                                    if (Decimal.TryParse(columnValue, out dc))
                                    {
                                        propertyInfo.SetValue(obj, dc, null);
                                    }

                                }
                                else if ((propertyInfo.PropertyType == typeof(bool)) || (propertyInfo.PropertyType == typeof(bool?)))
                                {
                                    // propertyType is boolean
                                    if ("1".Equals(columnValue) || "True".Equals(columnValue) || "Y".Equals(columnValue))
                                    {
                                        propertyInfo.SetValue(obj, true, null);
                                    }
                                    else
                                    {
                                        propertyInfo.SetValue(obj, false, null);
                                    }
                                }
                                else
                                {
                                    // propertyType is string
                                    propertyInfo.SetValue(obj, columnValue, null);
                                }

                            }
                            catch
                            {

                            }

                        }
                    }

                    converter.Add(obj);
                }
            }

            return converter;
        }

        public static object SetDisabledForField(bool notEdit = false)
        {
            var routeDic = new RouteValueDictionary();
            routeDic["readonly"] = "";
            return routeDic;
        }

        /// <summary>
        /// Mã hóa
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string EncryptStringMD5(string strSource)
        {
            string str_md5 = "";
            byte[] mang = System.Text.Encoding.UTF8.GetBytes(strSource);

            MD5CryptoServiceProvider my_md5 = new MD5CryptoServiceProvider();
            mang = my_md5.ComputeHash(mang);

            foreach (byte b in mang)
            {
                str_md5 += b.ToString("X2");
            }

            return str_md5;
        }

        public static string EncryptStringSHA256(string strSource)
        {
            return Cipher.Encrypt(strSource, Global.KeyMaHoa);
        }

        public static string DecryptStringSHA256(string strSource)
        {
            return Cipher.Decrypt(strSource, Global.KeyMaHoa);
        }

        public static string EncryptStringUTF8(string strSource)
        {
            string strResult = string.Empty;
            if (!string.IsNullOrWhiteSpace(strSource))
            {
                byte[] passBytes = System.Text.Encoding.Unicode.GetBytes(strSource);
                strResult = Convert.ToBase64String(passBytes);
            }
            return strResult;
        }


        /// <summary>
        /// Giải mã
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string DecryptStringUTF8(string strSource)
        {
            string strResult = string.Empty;
            if (!string.IsNullOrWhiteSpace(strSource))
            {
                byte[] passByteData = Convert.FromBase64String(strSource);
                strResult = System.Text.Encoding.Unicode.GetString(passByteData);
            }
            return strResult;
        }

        public static string DecimalToString(decimal value)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");   // try with "en-US"
            var result = "";
            result = value.ToString(Global.NumberFormat, cul);
            return result;
        }
    }
}
