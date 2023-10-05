using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_RESOURCE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string PROJECT_ROLE_ID { get; set; }
        public virtual string PROJECT_USER_TYPE_CODE { get; set; }
        public virtual DateTime? FROM_DATE { get; set; }
        public virtual DateTime? TO_DATE { get; set; }
        public virtual bool IS_SEND_MAIL { get; set; }

        public virtual T_AD_USER User { get; set; }
        public virtual T_PS_PROJECT Project { get; set; }
        public virtual T_MD_PROJECT_ROLE ProjectRole { get; set; }
        public virtual T_MD_PROJECT_USER_TYPE ProjectUserType { get; set; }

    }
}
