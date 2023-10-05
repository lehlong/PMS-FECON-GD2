using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_TASK_CHECK_LIST : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid TASK_ID { get; set; }
        public virtual string TEXT { get; set; }
        public virtual bool STATUS { get; set; }
    }
}
