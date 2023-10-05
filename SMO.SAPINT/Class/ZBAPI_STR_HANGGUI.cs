using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpSapRfc;

namespace SMO.SAPINT.Class
{
    public class ZBAPI_STR_HANGGUI
    {
        [RfcStructureField("BWTAR")]
        public string BATCH { get; set; }
        [RfcStructureField("MATNR")]
        public string MATERIAL_CODE { get; set; }
        [RfcStructureField("SMO")]
        public decimal QUANTITY { get; set; }
        [RfcStructureField("MEINS")]
        public string UNIT { get; set; }
        public decimal QUANTITY_CHUA_PD { get; set; }
    }
}
