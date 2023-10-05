using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.SAPINT
{
    public class PO_DETAIL
    {
        public string SMOID { get; set; }
        public int ITM_NUMBER { get; set; }
        public string MATERIAL { get; set; }
        public decimal TARGET_QTY { get; set; }
        public string SALES_UNIT { get; set; }
        public decimal MENGE_AP { get; set; }
    }
}
