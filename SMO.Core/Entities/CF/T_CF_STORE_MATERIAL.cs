using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CF_STORE_MATERIAL : BaseEntity
    {
        public virtual string STORE_CODE { get; set; }
        public virtual string MATERIAL_CODE { get; set; }
        public virtual T_MD_STORE Store { get; set; }
        public virtual T_MD_MATERIAL Material { get; set; }

        public T_CF_STORE_MATERIAL()
        {
            Store = new T_MD_STORE();
            Material = new T_MD_MATERIAL();
        }

        public override bool Equals(object obj)
        {
            var other = obj as T_CF_STORE_MATERIAL;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.STORE_CODE == other.STORE_CODE && this.MATERIAL_CODE == other.MATERIAL_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ STORE_CODE.GetHashCode();
                hash = (hash * 31) ^ MATERIAL_CODE.GetHashCode();
                return hash;
            }
        }

    }
}
