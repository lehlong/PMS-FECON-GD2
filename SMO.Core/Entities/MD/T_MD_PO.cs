using SharpSapRfc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_PO : BaseEntity
    {
        public virtual string PKID { get; set; }
        [RfcStructureField("EBELN")]
        public virtual string PO_NUMBER { get; set; }
        [RfcStructureField("EBELP")]
        public virtual string PO_ITEM { get; set; }
        [RfcStructureField("BSART")]
        public virtual string PO_TYPE { get; set; }
        [RfcStructureField("EKORG")]
        public virtual string PO_ORG { get; set; }
        [RfcStructureField("EKGRP")]
        public virtual string PO_GROUP { get; set; }
        [RfcStructureField("BEDAT")]
        public virtual DateTime PO_DATE { get; set; }
        [RfcStructureField("MATNR")]
        public virtual string MATERIAL_CODE { get; set; }
        [RfcStructureField("WERKS")]
        public virtual string PLANT_CODE { get; set; }
        [RfcStructureField("MENGE")]
        public virtual string QUANITY { get; set; }
        [RfcStructureField("MEINS")]
        public virtual string UNIT_CODE { get; set; }
        [RfcStructureField("POLOCK")]
        public virtual string PO_LOCK { get; set; }
    }
}
