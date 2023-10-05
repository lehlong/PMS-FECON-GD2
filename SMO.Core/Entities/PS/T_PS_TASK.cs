using System;
using System.Collections.Generic;

namespace SMO.Core.Entities.PS
{
    public class T_PS_TASK : BaseEntity
    {
        public T_PS_TASK()
        {
            CheckLists = new List<T_PS_TASK_CHECK_LIST>();
        }

        public virtual Guid ID { get; set; }
        public virtual Guid ACTIVITY_PARENT_ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string TEXT { get; set; }
        public virtual DateTime? START_DATE { get; set; }
        public virtual DateTime? FINISH_DATE { get; set; }
        public virtual Guid USER_PERFORMER { get; set; }
        public virtual Guid USER_APPROVE { get; set; }
        public virtual string DESCRIPTION { get; set; }
        public virtual string PRIORITY { get; set; }
        public virtual string STATUS { get; set; }

        public virtual IList<T_PS_TASK_CHECK_LIST> CheckLists { get; set; }
        public virtual T_PS_RESOURCE UserPerformer { get; set; }
        public virtual T_PS_RESOURCE UserAprove { get; set; }
    }
}
