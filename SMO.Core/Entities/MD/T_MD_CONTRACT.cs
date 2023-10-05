using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_CONTRACT : BaseEntity
    {
        [RfcStructureField("VBELN")]
        public virtual string CODE { get; set; }
        [RfcStructureField("AUART")]
        public virtual string CONTRACT_TYPE { get; set; }
        [RfcStructureField("VKORG")]
        public virtual string SALEORG { get; set; }
        [RfcStructureField("VTWEG")]
        public virtual string DC_CODE { get; set; }
        [RfcStructureField("SPART")]
        public virtual string DIVISION_CODE { get; set; }
        [RfcStructureField("KUNNR")]
        public virtual string CUSTOMER_CODE { get; set; }
        [RfcStructureField("GUEBG")]
        public virtual DateTime? VALID_FROM { get; set; }
        [RfcStructureField("GUEEN")]
        public virtual DateTime? VALID_TO { get; set; }
        [RfcStructureField("INCO1")]
        public virtual string INCOTERMS1 { get; set; }
        [RfcStructureField("INCO2")]
        public virtual string INCOTERMS2 { get; set; }
        [RfcStructureField("ZTERM")]
        public virtual string PAYMENTTERM_CODE { get; set; }
        [RfcStructureField("MATNR")]
        public virtual string MATERIAL_CODE { get; set; }
        [RfcStructureField("MEINS")]
        public virtual string UNIT { get; set; }
    }
}
