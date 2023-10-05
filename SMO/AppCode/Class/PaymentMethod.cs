using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO
{
    public static class PaymentMethod
    {
        public const string Befor = "01";
        public const string After = "02";

        public static string GetText(string type)
        {
            if (type == Befor)
            {
                return "Thanh toán trước";
            }
            else if (type == After)
            {
                return "Thanh toán sau";
            }
            else
            {
                return "";
            }
        }
    }
}