using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.PS
{
    [Serializable]
    public class T_PS_PROJECT_USER_RIGHT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string RIGHT_CODE { get; set; }
        public virtual bool IS_ADD { get; set; }
        public virtual bool IS_REMOVE { get; set; }
    }
}
