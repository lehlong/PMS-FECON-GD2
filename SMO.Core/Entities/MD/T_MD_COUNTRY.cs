using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_COUNTRY : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string TEXT { get; set; }
    }
}
