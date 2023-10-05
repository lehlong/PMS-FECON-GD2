using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.SAPINT
{
    public class SAP_INVOICE
    {
        public string VBELN { get; set; }
        public string POSNR { get; set; }
        public string SEQ_NMBR { get; set; }
        public string CUSNAME { get; set; }
        public string DIACHI { get; set; }
        public string MST { get; set; }
        public string MATNR { get; set; }
        public string SOHOADON { get; set; }
        public string LTT { get; set; }
        public string LTT_DES { get; set; }
        public string MATRACUU { get; set; }
        public string BWTAR { get; set; }
        [RfcStructureField("DATE_TH", "TIME_TH")]
        public DateTime? NGAY_TAO_HOA_DON { get; set; }
        public string ETYPE { get; set; }
        public string SERI_PHOI { get; set; }
    }
}
