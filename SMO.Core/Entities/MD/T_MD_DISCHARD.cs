using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_DISCHARD : BaseEntity
    {
        [RfcStructureField("KNOTE")]
        public virtual string CODE { get; set; }
        [RfcStructureField("BEZEI")]
        public virtual string TEXT { get; set; }
        public virtual string CUSTOMER_CODE { get; set; }
    }
}
