using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public static class ThoiGianGiaoHang
    {
        public const string Ngay = "1";
        public const string Dem = "2";

        public static string GetText(string type)
        {
            if (type == Ngay)
            {
                return "Ngày";
            }

            else if (type == Dem)
            {
                return "Đêm";
            }
            return "";
        }
    }
}