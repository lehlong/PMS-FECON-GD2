using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMO.SAPINT;
using SAP.Middleware.Connector;

namespace SMO
{
    public static class Global
    {
        public static SAPDestinationConfig DestinationConfig = new SAPDestinationConfig();
        public const string ApplicationName = "SMO";
        public const string LanguageDefault = "vi";
        public const string DateSAPFormat = "yyyy-MM-dd";
        public const string NumberFormat = "#,#0.##";
        public const string NumberFormat6 = "#,#0.######";
        public const string DateToStringFormat = "dd/MM/yyyy";
        public const string DateToStringMomentJSFormat = "DD/MM/YYYY";
        public const string DateTimeToStringFormat = "dd/MM/yyyy HH:mm";
        public const string KeyMaHoa = "D2s@1234!@#";

        public static Dictionary<string, int> ListIPLogin = new Dictionary<string, int>();
    }
}