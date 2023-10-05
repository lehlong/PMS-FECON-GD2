using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO.Service.Class
{
    public class RESULT_EGAS
    {
        public bool STATUS { get; set; }
        public List<EGAS_HEADER> DATA { get; set; }
        public string MESSAGE { get; set; }
    }
}