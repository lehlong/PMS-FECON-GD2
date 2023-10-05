using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_CM_REFERENCE_LINK : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid REFERENCE_ID { get; set; }
        public virtual string LINK { get; set; }
    }
}
