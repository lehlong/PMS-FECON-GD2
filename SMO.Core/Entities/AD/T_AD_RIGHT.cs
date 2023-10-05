using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_RIGHT : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string PARENT { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual IList<T_AD_ROLE_DETAIL> ListRoleDetail { get; set; }
        public virtual IList<T_AD_USER_RIGHT> ListUserRight { get; set; }

        public T_AD_RIGHT()
        {
            ListRoleDetail = new List<T_AD_ROLE_DETAIL>();
            ListUserRight = new List<T_AD_USER_RIGHT>();
        }
    }
}
