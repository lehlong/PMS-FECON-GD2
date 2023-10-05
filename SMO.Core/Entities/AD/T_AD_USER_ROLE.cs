using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_ROLE : BaseEntity
    {
        public virtual string ROLE_CODE { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual T_AD_ROLE Role { get; set; }
        public virtual T_AD_USER User { get; set; }

        public T_AD_USER_ROLE()
        {
            Role = new T_AD_ROLE();
            User = new T_AD_USER();
        }

        public override bool Equals(object obj)
        {
            var other = obj as T_AD_USER_ROLE;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.ROLE_CODE == other.ROLE_CODE && this.USER_NAME == other.USER_NAME;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ ROLE_CODE.GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                return hash;
            }
        }
    }
}
