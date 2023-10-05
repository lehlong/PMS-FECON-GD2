using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_PROJECT_STRUCT_SAP : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid PROJECT_STRUCT_ID { get; set; }
        public virtual string ACTION { get; set; }
    }
}
