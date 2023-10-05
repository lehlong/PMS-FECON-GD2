using SMO.Core.Entities;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_REFERENCE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid SOURCE_ID { get; set; }
        public virtual Guid TARGET_ID { get; set; }
        public virtual string TYPE { get; set; }
    }
}
