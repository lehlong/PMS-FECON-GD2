using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.SAPINT
{
    public class VEHICLE_COMPARTMENT
    {
        [RfcStructureField("VEHICLE")]
        public string VEHICLE_CODE { get; set; }
        [RfcStructureField("VEH_TYPE")]
        public string TRANSUNIT_CODE { get; set; }
        [RfcStructureField("VOL_UOM")]
        public string UNIT { get; set; }
        [RfcStructureField("SEQ_NMBR")]
        public string SEQ_NUMBER { get; set; }
        [RfcStructureField("CMP_MAXVOL")]
        public decimal CAPACITY { get; set; }
        [RfcStructureField("OIC_PBATCH")]
        public string OIC_PBATCH { get; set; }
        [RfcStructureField("OIC_PTRIP")]
        public string OIC_PTRIP { get; set; }
        [RfcStructureField("OIC_MOT")]
        public string TRANSMODE_CODE { get; set; }
    }
}
