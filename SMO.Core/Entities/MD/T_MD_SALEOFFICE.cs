using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_SALEOFFICE : BaseEntity
    {
        [RfcStructureField("VKBUR")]
        public virtual string CODE { get; set; }
        [RfcStructureField("BEZEI")]
        public virtual string TEXT { get; set; }
        [RfcStructureField("VKORG")]
        public virtual string COMPANY_CODE { get; set; }
        public virtual string DISCHARD_POINT { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string PHONE { get; set; }
        public virtual string DESCRIPTION { get; set; }
    }
}
