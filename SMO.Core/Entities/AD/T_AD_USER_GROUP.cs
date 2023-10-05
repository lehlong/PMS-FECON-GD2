using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_GROUP : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string NOTES { get; set; }
#pragma warning disable CS0114 // 'T_AD_USER_GROUP.ACTIVE' hides inherited member 'BaseEntity.ACTIVE'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public virtual bool ACTIVE { get; set; }
#pragma warning restore CS0114 // 'T_AD_USER_GROUP.ACTIVE' hides inherited member 'BaseEntity.ACTIVE'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public virtual IList<T_AD_USER_USER_GROUP> ListUserUserGroup { get; set; }
        public virtual IList<T_AD_USER_GROUP_ROLE> ListUserGroupRole { get; set; }

        public T_AD_USER_GROUP()
        {
            ListUserUserGroup = new List<T_AD_USER_USER_GROUP>();
            ListUserGroupRole = new List<T_AD_USER_GROUP_ROLE>();
        }
    }
}
