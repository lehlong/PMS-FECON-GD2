using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public static class UserType
    {
        /// <summary>
        /// User khách hàng
        /// </summary>
        public const string Fecon = "KH";
        /// <summary>
        /// User cửa hàng
        /// </summary>
        public const string Vendor = "VD";

        public static string GetText(string type)
        {
            if (type == Fecon)
            {
                return "Fecon";
            }

            else if (type == Vendor)
            {
                return "Nhà thầu";
            }
            else
            {
                return "";
            }
        }
    }
}