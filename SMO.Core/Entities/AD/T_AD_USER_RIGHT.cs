using System;
using System.Collections;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_RIGHT : BaseEntity
    {
        public virtual string USER_NAME { get; set; }
        public virtual string FK_RIGHT { get; set; }
        public virtual bool IS_ADD { get; set; }
        public virtual bool IS_REMOVE { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as T_AD_USER_RIGHT;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.USER_NAME == other.USER_NAME && this.FK_RIGHT == other.FK_RIGHT;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                hash = (hash * 31) ^ FK_RIGHT.GetHashCode();
                return hash;
            }
        }
    }
}
