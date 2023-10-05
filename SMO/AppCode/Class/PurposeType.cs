using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public static class PurposeType
    {
        public const string CuaHangKho = "01";
        public const string BanChuyenThang = "02";

        public static string GetText(string type)
        {
            if (type == CuaHangKho)
            {
                return "Di chuyển về cửa hàng/kho";
            }

            else if (type == BanChuyenThang)
            {
                return "Bán chuyển thẳng";
            }
            return "";
        }
    }
}