using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_VENDOR : BaseEntity
    {
        public virtual string VENDOR_CODE { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual T_MD_VENDOR_OLD Vendor { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as T_AD_USER_VENDOR;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.VENDOR_CODE == other.VENDOR_CODE && this.USER_NAME == other.USER_NAME;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ VENDOR_CODE.GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                return hash;
            }
        }
    }
}
