using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_CUSTOMER : BaseEntity
    {
        public virtual string CUSTOMER_CODE { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string COMPANY_CODE { get; set; }
        public virtual T_MD_CUSTOMER_OLD Customer { get; set; }

        //public T_AD_USER_CUSTOMER()
        //{
        //    Customer = new T_MD_CUSTOMER_OLD();
        //}

        public override bool Equals(object obj)
        {
            var other = obj as T_AD_USER_CUSTOMER;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.CUSTOMER_CODE == other.CUSTOMER_CODE && this.USER_NAME == other.USER_NAME && this.COMPANY_CODE == other.COMPANY_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ CUSTOMER_CODE.GetHashCode();
                hash = (hash * 31) ^ USER_NAME.GetHashCode();
                hash = (hash * 31) ^ COMPANY_CODE.GetHashCode();
                return hash;
            }
        }
    }
}
