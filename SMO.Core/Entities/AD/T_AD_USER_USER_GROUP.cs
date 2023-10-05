using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_USER_GROUP : BaseEntity
    {
        public virtual string USER_NAME { get; set; }
        public virtual string USER_GROUP_CODE { get; set; }
        public virtual T_AD_USER User { get; set; }
        public virtual T_AD_USER_GROUP UserGroup { get; set; }

        public T_AD_USER_USER_GROUP()
        {
            User = new T_AD_USER();
            UserGroup = new T_AD_USER_GROUP();
        }

        public override bool Equals(object obj)
        {
            var other = obj as T_AD_USER_USER_GROUP;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.USER_NAME == other.USER_NAME && this.USER_GROUP_CODE == other.USER_GROUP_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                hash = (hash * 31) ^ USER_GROUP_CODE.GetHashCode();
                return hash;
            }
        }
    }
}
