using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_GROUP_ROLE : BaseEntity
    {
        public virtual string ROLE_CODE { get; set; }
        public virtual string USER_GROUP_CODE { get; set; }
        public virtual T_AD_ROLE Role { get; set; }
        public virtual T_AD_USER_GROUP UserGroup { get; set; }

        public T_AD_USER_GROUP_ROLE()
        {
            Role = new T_AD_ROLE();
            UserGroup = new T_AD_USER_GROUP();
        }

        public override bool Equals(object obj)
        {
            var other = obj as T_AD_USER_GROUP_ROLE;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.ROLE_CODE == other.ROLE_CODE && this.USER_GROUP_CODE == other.USER_GROUP_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ ROLE_CODE.GetHashCode();
                hash = (hash * 31) ^ USER_GROUP_CODE.GetHashCode();
                return hash;
            }
        }
    }
}
