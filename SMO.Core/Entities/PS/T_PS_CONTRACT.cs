using SMO.Core.Entities.MD;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONTRACT : BaseEntity
    {
        public T_PS_CONTRACT()
        {
            Details = new List<T_PS_CONTRACT_DETAIL>();
        }
        public virtual Guid ID { get; set; }
        public virtual Guid? PARENT_CODE { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string NAME { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string CONTRACT_NUMBER { get; set; }
        public virtual string CONTRACT_TYPE { get; set; }
        public virtual string PO_SO_NUMBER { get; set; }
        public virtual bool IS_SIGN_WITH_CUSTOMER { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string VENDOR_CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string CUSTOMER_CODE { get; set; }
        public virtual string PAYMENT_STATUS { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual DateTime? START_DATE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual DateTime? FINISH_DATE { get; set; }
        public virtual DateTime? EXTEND_DATE { get; set; }
        public virtual decimal CONTRACT_VALUE { get; set; }
        public virtual decimal CONTRACT_VALUE_VAT { get; set; }
        public virtual decimal VAT { get; set; }
        public virtual string NOTES { get; set; }
        public virtual string REPRESENT_A { get; set; }
        public virtual string REPRESENT_B { get; set; }
        public virtual string NGUOI_PHU_TRACH { get; set; }
        public virtual Guid? REFERENCE_FILE_ID { get; set; }
        public virtual IList<T_PS_CONTRACT_DETAIL> Details { get; set; }

        private T_MD_PAYMENT_STATUS _PaymentStatus;

        public virtual T_MD_PAYMENT_STATUS PaymentStatus
        {
            get
            {
                try
                {
                    if (this._PaymentStatus != null)
                    {
                        var text = this._PaymentStatus.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._PaymentStatus;
            }
            set
            {
                this._PaymentStatus = value;
            }
        }

        private T_PS_CONTRACT _ParentContract;

        public virtual T_PS_CONTRACT ParentContract
        {
            get
            {
                try
                {
                    if (this._ParentContract != null)
                    {
                        var text = this._ParentContract.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._ParentContract;
            }
            set
            {
                this._ParentContract = value;
            }
        }

        private T_PS_PROJECT _Project;

        public virtual T_PS_PROJECT Project
        {
            get
            {
                try
                {
                    if (this._Project != null)
                    {
                        var text = this._Project.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Project;
            }
            set
            {
                this._Project = value;
            }
        }

        private T_MD_CONTRACT_TYPE _ContractType;

        public virtual T_MD_CONTRACT_TYPE ContractType
        {
            get
            {
                try
                {
                    if (this._ContractType != null)
                    {
                        var text = this._ContractType.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._ContractType;
            }
            set
            {
                this._ContractType = value;
            }
        }

        private T_MD_VENDOR _Vendor;

        public virtual T_MD_VENDOR Vendor
        {
            get
            {
                try
                {
                    if (this._Vendor != null)
                    {
                        var text = this._Vendor.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Vendor;
            }
            set
            {
                this._Vendor = value;
            }
        }

        private T_MD_CUSTOMER _Customer;

        public virtual T_MD_CUSTOMER Customer
        {
            get
            {
                try
                {
                    if (this._Customer != null)
                    {
                        var text = this._Customer.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Customer;
            }
            set
            {
                this._Customer = value;
            }
        }
    }
}
