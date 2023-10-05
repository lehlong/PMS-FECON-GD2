using System;
using System.Collections;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_ROLE_DETAIL : BaseEntity
    {
        public virtual string FK_ROLE { get; set; }
        public virtual string FK_RIGHT { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as T_AD_ROLE_DETAIL;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.FK_ROLE == other.FK_ROLE && this.FK_RIGHT == other.FK_RIGHT;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ FK_ROLE.GetHashCode();
                hash = (hash * 31) ^ FK_RIGHT.GetHashCode();
                return hash;
            }
        }
    }
}
