using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_PLAN_VOLUME_DESIGN : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid PROJECT_STRUCT_ID { get; set; }
        public virtual Guid? CONTRACT_DETAIL_ID { get; set; }
        public virtual decimal VALUE { get; set; }
    }
}
