using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONFIG_HIDE_COLUMN : BaseEntity
    {
        public virtual string USER_NAME { get; set; }
        public virtual string TYPE_DISPLAY { get; set; }
        public virtual string DETAILS { get; set; }
        public virtual Guid PROJECT_ID { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as T_PS_CONFIG_HIDE_COLUMN;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.USER_NAME == other.USER_NAME;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                return hash;
            }
        }

    }
}
