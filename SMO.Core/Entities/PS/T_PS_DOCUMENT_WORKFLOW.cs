using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SMO.Core.Entities.MD;

namespace SMO.Core.Entities.PS
{
    public class T_PS_DOCUMENT_WORKFLOW : BaseEntity
    {
        public virtual Guid ID { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [MaxLength(length: 50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string NAME { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string REQUEST_TYPE_CODE { get; set; }
        public virtual string PROJECT_LEVEL_CODE { get; set; }
        public virtual decimal CONTRACT_VALUE_MIN { get; set; }
        public virtual decimal CONTRACT_VALUE_MAX { get; set; }
        public virtual string PURCHASE_TYPE_CODE { get; set; }
        public virtual bool AUTHORITY { get; set; }

        public virtual Guid DOCUMENT_ID { get; set; }
        public virtual ISet<T_PS_DOCUMENT_WORKFLOW_FILE> ListFiles { get; set; }
        public virtual ISet<T_PS_DOCUMENT_WORKFLOW_STEP> ListSteps { get; set; }

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

        private T_MD_PROJECT_LEVEL _ProjectLevel;
        public virtual T_MD_PROJECT_LEVEL ProjectLevel
        {
            get
            {
                try
                {
                    if (this._ProjectLevel != null)
                    {
                        var text = this._ProjectLevel.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._ProjectLevel;
            }
            set
            {
                this._ProjectLevel = value;
            }
        }

        private T_MD_PURCHASE_TYPE _PurchaseType;
        public virtual T_MD_PURCHASE_TYPE PurchaseType
        {
            get
            {
                try
                {
                    if (this._PurchaseType != null)
                    {
                        var text = this._PurchaseType.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._PurchaseType;
            }
            set
            {
                this._PurchaseType = value;
            }
        }
        public T_PS_DOCUMENT_WORKFLOW()
        {
            ListFiles = new HashSet<T_PS_DOCUMENT_WORKFLOW_FILE>(new List<T_PS_DOCUMENT_WORKFLOW_FILE>());
            ListSteps = new HashSet<T_PS_DOCUMENT_WORKFLOW_STEP>(new List<T_PS_DOCUMENT_WORKFLOW_STEP>());
        }
    }
}
