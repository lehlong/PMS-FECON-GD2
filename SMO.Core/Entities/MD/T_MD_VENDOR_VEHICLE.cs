using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_VENDOR_VEHICLE : BaseEntity
    {
        public virtual string VENDOR_CODE { get; set; }
        public virtual string VEHICLE_CODE { get; set; }

        public virtual T_MD_VENDOR_OLD Vendor { get; set; }
        public virtual T_MD_VEHICLE Vehicle { get; set; }

        public T_MD_VENDOR_VEHICLE()
        {
            Vendor = new T_MD_VENDOR_OLD();
            Vehicle = new T_MD_VEHICLE();
        }

        public override bool Equals(object obj)
        {
            var other = obj as T_MD_VENDOR_VEHICLE;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.VENDOR_CODE == other.VENDOR_CODE && this.VEHICLE_CODE == other.VEHICLE_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ VENDOR_CODE.GetHashCode();
                hash = (hash * 31) ^ VEHICLE_CODE.GetHashCode();
                return hash;
            }
        }
    }
}
