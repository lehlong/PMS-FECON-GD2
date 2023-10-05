using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_CM_REFERENCE_FILE : BaseEntity
    {
        public virtual Guid REFERENCE_ID { get; set; }
        public virtual Guid FILE_ID { get; set; }
        public virtual T_CM_FILE_UPLOAD FileUpload { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as T_CM_REFERENCE_FILE;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.REFERENCE_ID == other.REFERENCE_ID && this.FILE_ID == other.FILE_ID;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ REFERENCE_ID.GetHashCode();
                hash = (hash * 31) ^ FILE_ID.GetHashCode();
                return hash;
            }
        }
    }
}
