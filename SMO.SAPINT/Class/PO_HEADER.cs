using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.SAPINT
{
    public class PO_HEADER
    {
        public string SMOID { get; set; }
        public string BUKRS_SMO { get; set; }
        public string BUKRS { get; set; }
        public string WERKS { get; set; }
        public string PARTN_NUMB { get; set; }
        public string VKBUR { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public DateTime? LFDAT { get; set; }
        public DateTime? ZZ_EXDA { get; set; }
        public string OIC_PBATCH { get; set; }
        public string OIC_PTRIP { get; set; }
        public string OIC_MOT { get; set; }
        public string OIC_TRUCKN { get; set; }
        public string DO_PROCEID { get; set; }
        public string FLG_STATUS { get; set; }
        public string KNOTZ { get; set; }
        public string REASON { get; set; }
        public string NOTE { get; set; }
        public string EBELN_STO { get; set; }
        public string SHNUMBER { get; set; }
        public string VBELN_DO { get; set; }
        public string VBELN_DO1 { get; set; }
        public string VBELV_SO { get; set; }
        public string RECEIPT_POINT { get; set; }
        public string HOSTADDR { get; set; }
        public string USERSMO { get; set; }
        public string UPDATEFLAG { get; set; }
        public string APSTATUS { get; set; }
        public string UNAME_APP { get; set; }
        [RfcStructureField("DATUM_APP", "UZEIT_APP")]
        public DateTime? NGAY_PHE_DUYET { get; set; }
        public string GET_TO_SMO { get; set; }
        public string EVT_FLG { get; set; }

        public string NGUON { get; set; }
        public string PT_XUAT { get; set; }
        public string PT_VC { get; set; }
        public string KHO_XUAT { get; set; }
        public string DV_NHAN { get; set; }
        public string DV_VANTAI { get; set; }
        public string SOPTIEN { get; set; }
        public string NGUOIVC { get; set; }
        public string SO_TKTN { get; set; }
        public string OIC_LIFNR { get; set; }

        public DateTime? LFDATE { get; set; }
        public string NGAY_DEM { get; set; }
        public string NGUOIDD { get; set; }
        public string REG_CNTRY { get; set; }
        public string TEN_PT { get; set; }
        public string DIEMGIAO { get; set; }
        public string TK_NUMBER { get; set; }
        public string PT_THANHTTOAN { get; set; }
        public string INCOTERMS1 { get; set; }
        public string MD_DC { get; set; }
        public string NOTE_LX { get; set; }
        public DateTime? DOC_DATE { get; set; }
    }
}
