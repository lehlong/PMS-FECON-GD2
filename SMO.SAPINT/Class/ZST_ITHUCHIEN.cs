using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSapRfc;

namespace SMO.SAPINT.Class
{
    public class ZST_ITHUCHIEN
    {
        public DateTime? ZDATE { get; set; }
        public string H_TEXT { get; set; }
        public string ZPS01 { get; set; }
        public string ZPS03 { get; set; }
        public string RECOPERATN { get; set; }
        public string REC_WBS_EL { get; set; }
        public string STATKEYFIG { get; set; }
        public decimal STAT_QTY { get; set; }
    }
}
