using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO.Service.Class
{
    public class EGAS_DETAIL
    {
        public string SO_CT { get; set; }
        public DateTime NGAY_CT { get; set; }
        public string MATERIAL { get; set; }
        public string UNIT { get; set; }
        public decimal QUANTITY_LTT { get; set; }
        public decimal QUANTITY_L15 { get; set; }
    }
}