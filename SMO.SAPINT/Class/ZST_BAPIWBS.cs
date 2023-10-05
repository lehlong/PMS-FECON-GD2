using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSapRfc;

namespace SMO.SAPINT.Class
{
    public class ZST_BAPIWBS
    {
        [RfcStructureField("POSID")]
        public string WBS_CODE { get; set; }
        [RfcStructureField("POST1")]
        public string WBS_NAME { get; set; }
        [RfcStructureField("WBS_CHA")]
        public string PARENT_CODE { get; set; }
        [RfcStructureField("PSTRT")]
        public DateTime? START_DATE { get; set; }
        [RfcStructureField("PENDE")]
        public DateTime? FINISH_DATE { get; set; }
        [RfcStructureField("ISTRT")]
        public DateTime? ACTUAL_START_DATE { get; set; }
        [RfcStructureField("IENDE")]
        public DateTime? ACTUAL_FINISH_DATE { get; set; }
    }
}
