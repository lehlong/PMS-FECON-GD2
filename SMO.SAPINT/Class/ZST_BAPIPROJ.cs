using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSapRfc;

namespace SMO.SAPINT.Class
{
    public class ZST_BAPIPROJ
    {
        [RfcStructureField("PROJ")]
        public string PROJECT_CODE { get; set; }
        [RfcStructureField("POST1")]
        public string PROJECT_NAME { get; set; }
        [RfcStructureField("VBUKR")]
        public string COMPANY_CODE { get; set; }
        [RfcStructureField("PLFAZ")]
        public DateTime? START_DATE { get; set; }
        [RfcStructureField("PLSEZ")]
        public DateTime? FINISH_DATE { get; set; }
        [RfcStructureField("PROFI")]
        public string PROJECT_PROFILE { get; set; }
        [RfcStructureField("KOSTL")]
        public string COST_CENTER_CODE { get; set; }
    }
}
