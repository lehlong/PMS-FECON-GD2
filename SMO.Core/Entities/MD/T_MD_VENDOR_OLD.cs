using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_VENDOR_OLD : BaseEntity
    {
        [RfcStructureField("VENDORCODE")]
        public virtual string CODE { get; set; }
        [RfcStructureField("VENDORNAME")]
        public virtual string TEXT { get; set; }
    }
}
