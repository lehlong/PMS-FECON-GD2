using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO.Service.Class
{
    public class EGAS_HEADER
    {
        public string TD_NUMBER { get; set; }
        public string SALEOFFICE { get; set; }
        public List<EGAS_DETAIL> LIST_MATERIAL { get; set; }
    }
}