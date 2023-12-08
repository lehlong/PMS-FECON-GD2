using SMO.Core.Entities.MD;
using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONTRACT_REQUEST_PAYMENT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid CONTRACT_ID { get; set; }
        public virtual decimal AMOUNT { get; set; }
        public virtual string CURRENCY_CODE { get; set; }
        [Required(ErrorMessage = "Ngày thanh toán bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual DateTime PAYMENT_DATE { get; set; }
        public virtual string BILL_NUMBER { get; set; }
        public virtual decimal AMOUNT_ADVANCE { get; set; }
        public virtual decimal INVOICE_VALUE { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual string EXPLAIN { get; set; }
        public virtual string STATUS { get; set; }
        public virtual Guid REFERENCE_FILE_ID { get; set; }
        public virtual Guid WORKFLOW_ID { get; set; }
        public virtual string REQUEST_TYPE_CODE { get; set; }
        public virtual decimal EXCHANGE_RATE { get; set; }

        public virtual T_MD_PAYMENT_STATUS PaymentStatus { get; set; }

        private T_MD_REQUEST_TYPE _RequestType;
        public virtual T_MD_REQUEST_TYPE RequestType
        {
            get
            {
                try
                {
                    if (this._RequestType != null)
                    {
                        var text = this._RequestType.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._RequestType;
            }
            set
            {
                this._RequestType = value;
            }
        }

        private T_PS_DOCUMENT_WORKFLOW _Workflow;
        public virtual T_PS_DOCUMENT_WORKFLOW Workflow
        {
            get
            {
                try
                {
                    if (this._Workflow != null)
                    {
                        var text = this._Workflow.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Workflow;
            }
            set
            {
                this._Workflow = value;
            }
        }
    }
}
