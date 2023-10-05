using System;

namespace SMO.Core.Entities.MD
{
    public class T_MD_PROJECT_ROLE_RIGHT : BaseEntity
    {
        public virtual string ID { get; set; }
        public virtual string PROJECT_ROLE_ID { get; set; }
        public virtual string RIGHT_CODE { get; set; }
    }
}
