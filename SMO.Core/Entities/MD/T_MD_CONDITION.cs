using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_CONDITION : BaseEntity
    {
        [RfcStructureField("KSCHL")]
        public virtual string CODE { get; set; }
        [RfcStructureField("VTEXT")]
        public virtual string NAME { get; set; }
        [RfcStructureField("KMANU")]
        public virtual string TYPE { get; set; }
        [RfcStructureField("KBETR")]
        public virtual string AMOUNT { get; set; }
        [RfcStructureField("WAERS")]
        public virtual string CURRENCY { get; set; }
        [RfcStructureField("KPEIN")]
        public virtual string PER { get; set; }
        [RfcStructureField("KMEIN")]
        public virtual string UNIT { get; set; }
        [RfcStructureField("MATNR")]
        public virtual string MATERIAL_CODE { get; set; }
        [RfcStructureField("KPOSN")]
        public virtual string ITEM_NUMBER { get; set; }
        [RfcStructureField("STUNR")]
        public virtual string STEP_NUMBER { get; set; }
        [RfcStructureField("ZAEHK")]
        public virtual string CONDITION_NUMBER { get; set; }
        public virtual bool MHGL_ALLOW_EDIT { get; set; }
        public virtual bool MHGL_ALLOW_SHOW { get; set; }
        public virtual bool XBND_ALLOW_EDIT { get; set; }
        public virtual bool XBND_ALLOW_SHOW { get; set; }
        public virtual bool XBTX_ALLOW_EDIT { get; set; }
        public virtual bool XBTX_ALLOW_SHOW { get; set; }
        public virtual bool TEST_ALLOW_EDIT { get; set; }
        public virtual bool TEST_ALLOW_SHOW { get; set; }
    }
}
