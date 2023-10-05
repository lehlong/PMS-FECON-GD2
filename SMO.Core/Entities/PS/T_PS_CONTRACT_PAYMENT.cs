using SMO.Core.Entities.MD;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONTRACT_PAYMENT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid CONTRACT_ID { get; set; }
        public virtual decimal AMOUNT { get; set; }
        public virtual string CURRENCY_CODE { get; set; }
        [Required (ErrorMessage = "Ngày thanh toán bắt buộc nhập", AllowEmptyStrings =false)]
        public virtual DateTime PAYMENT_DATE { get; set; }
        public virtual string BILL_NUMBER { get; set; }
        public virtual decimal AMOUNT_ADVANCE { get; set; }
        public virtual decimal INVOICE_VALUE { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual string EXPLAIN { get; set; }
        public virtual string STATUS { get; set; }
        public virtual Guid REFERENCE_FILE_ID { get; set; }
        public virtual T_MD_PAYMENT_STATUS PaymentStatus { get; set; }
    }
}
