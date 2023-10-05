using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public static class ModulType
    {
        public const string DCCH = "DCCH";
        public const string DCNB = "DCNB";
        public const string XBTX = "XBTX";
        public const string XBND = "XBND";
        public const string XTHG = "XTHG";
        public const string MHGL = "MHGL";
        public const string HHK  = "HHK";
        public const string KHLH = "KHLH";
        public const string SUM = "SUM";
        public const string TRANSFER = "TRANSFER";

        public static string GetBusinessType(string type, string companyCode = "")
        {
            if (type == DCCH)
            {
                return "01";
            }
            else if (type == DCNB)
            {
                return "05";
            }
            else if (type == XBTX)
            {
                return "04";
            }
            else if (type == XBND)
            {
                if (companyCode == "6630")
                {
                    return "02";
                }
                return "03";
            }
            else if (type == XTHG)
            {
                return "06";
            }
            else if (type == MHGL)
            {
                return "07";
            }
            return "";
        }

        public static string GetBusinessTypeSAP(string type, string companyCode = "")
        {
            if (type == DCCH)
            {
                return "02";
            }
            else if (type == DCNB)
            {
                return "03";
            }
            else if (type == XBTX)
            {
                return "01";
            }
            else if (type == XBND)
            {
                if (companyCode == "6630")
                {
                    return "06";
                }
                return "01";
            }
            else if (type == XTHG)
            {
                return "05";
            }
            else if (type == MHGL)
            {
                return "04";
            }
            return "";
        }

        public static string GetHeaderCode(string type)
        {
            if (type == DCCH)
            {
                return "1";
            }
            else if (type == DCNB)
            {
                return "5";
            }
            else if (type == XBTX)
            {
                return "4";
            }
            else if (type == XBND)
            {
                return "3";
            }
            else if (type == XTHG)
            {
                return "6";
            }
            else if (type == MHGL)
            {
                return "7";
            }
            else if (type == HHK)
            {
                return "8";
            }
            else if (type == KHLH)
            {
                return "9";
            }
            else if (type == SUM)
            {
                return "S";
            }
            else if (type == TRANSFER)
            {
                return "T";
            }
            return "";
        }

        public static string GetText(string type)
        {
            if (type == DCCH)
            {
                return "Di chuyển ra cửa hàng";
            }
            else if (type == DCNB)
            {
                return "Di chuyển nội bộ ngành";
            }
            else if (type == XBTX)
            {
                return "Xuất bán tái xuất";
            }
            else if (type == XBND)
            {
                return "Xuất bán nội địa";
            }
            else if (type == XTHG)
            {
                return "Xuất trả hàng gửi";
            }
            else if (type == MHGL)
            {
                return "Mua hàng gửi lại";
            }
            else if (type == HHK)
            {
                return "Hàng hóa khác";
            }
            else if (type == KHLH)
            {
                return "Kế hoạch lấy hàng";
            }
            else if (type == SUM)
            {
                return "Đơn hàng tổng";
            }
            return string.Empty;
        }
    }
}