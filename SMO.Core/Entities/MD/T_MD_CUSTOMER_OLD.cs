using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_CUSTOMER_OLD : BaseEntity
    {
        [RfcStructureField("KUNNR")]
        public virtual string CUSTOMER_CODE { get; set; }
        [Required(ErrorMessage ="Hãy nhập tên khách hàng!")]
        [RfcStructureField("NAME1")]
        public virtual string TEXT { get; set; }
        [RfcStructureField("STRAS")]
        public virtual string ADDRESS { get; set; }
        [RfcStructureField("STCEG")]
        [Required(ErrorMessage = "Nhập mã số thuế")]
        public virtual string VAT_NUMBER { get; set; }
        [RfcStructureField("EMAIL")]
        public virtual string EMAIL { get; set; }
        [RfcStructureField("PHONE")]
        public virtual string PHONE { get; set; }
        [RfcStructureField("BUKRS")]
        [Required(ErrorMessage ="Hãy chọn công ty!")]
        public virtual string COMPANY_CODE { get; set; }
        public virtual bool IS_SEND_ONLY_REJECT { get; set; }



        public override bool Equals(object obj)
        {
            var other = obj as T_MD_CUSTOMER_OLD;

            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return this.CUSTOMER_CODE == other.CUSTOMER_CODE && this.COMPANY_CODE == other.COMPANY_CODE;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                if (CUSTOMER_CODE == null)
                {
                    CUSTOMER_CODE = "";
                }

                if (COMPANY_CODE == null)
                {
                    COMPANY_CODE = "";
                }
                int hash = GetType().GetHashCode();
                hash = (hash * 31) ^ CUSTOMER_CODE.GetHashCode();
                hash = (hash * 31) ^ COMPANY_CODE.GetHashCode();
                return hash;
            }
        }
    }
}
