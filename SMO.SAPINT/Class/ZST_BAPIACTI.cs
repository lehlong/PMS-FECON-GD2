using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSapRfc;

namespace SMO.SAPINT.Class
{
    public class ZST_BAPIACTI
    {
        [RfcStructureField("ACTIVITY")]
        public string ACTIVITY_CODE { get; set; }
        [RfcStructureField("LTXA1")]
        public string ACTIVITY_NAME { get; set; }
        
        [RfcStructureField("WBS_CHA")]
        public string PARENT_CODE { get; set; }
        [RfcStructureField("LOSME")]
        public string UNIT_CODE { get; set; }
        [RfcStructureField("LOSVG")]
        public decimal? QUANTITY { get; set; }
        [RfcStructureField("STEUS")]
        public string CONTROL_KEY { get; set; }
        [RfcStructureField("PURCH_ORG")]
        public string PURCH_ORG { get; set; }
        [RfcStructureField("PUR_GROUP")]
        public string PUR_GROUP { get; set; }
        [RfcStructureField("FSAVD")]
        public DateTime? START_DATE { get; set; }
        [RfcStructureField("FSEDD")]
        public DateTime? FINISH_DATE { get; set; }
        [RfcStructureField("ISDD")]
        public DateTime? ACTUAL_START_DATE { get; set; }
        [RfcStructureField("IEDD")]
        public DateTime? ACTUAL_FINISH_DATE { get; set; }
    }
}
