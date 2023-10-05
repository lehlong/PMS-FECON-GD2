using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [MaxLength(length: 20, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        public virtual string USER_NAME { get; set; }

        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{10,}$", ErrorMessage = "Mật khẩu có ít nhất 10 kí tự. Bao gồm cả số,chữ thường và chữ hoa.")]
        public virtual string PASSWORD { get; set; }

        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{10,}$", ErrorMessage = "Mật khẩu có ít nhất 10 kí tự. Bao gồm cả số,chữ thường và chữ hoa.")]
        public virtual string RETRY_PASSWORD { get; set; }

        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string OLD_PASSWORD { get; set; }

        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string FULL_NAME { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string ACCOUNT_AD { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string ADDRESS { get; set; }
        public virtual string PHONE { get; set; }
        public virtual string NOTES { get; set; }
        public virtual string LANGUAGE { get; set; }
        public virtual string USER_TYPE { get; set; }
        public virtual string TITLE_CODE { get; set; }
        public virtual string COMPANY_ID { get; set; }
        public virtual bool IS_MODIFY_RIGHT { get; set; }
        public virtual bool OTP_VERIFY { get; set; }
        public virtual bool IS_IGNORE_USER { get; set; }
        public virtual string VENDOR_CODE { get; set; }
        public virtual string USER_SAP { get; set; }
        public virtual string PASSWORD_SAP { get; set; }
        public virtual DateTime? LAST_CHANGE_PASS_DATE { get; set; }
        public virtual ISet<T_AD_USER_USER_GROUP> ListUserUserGroup { get; set; }
        public virtual ISet<T_AD_USER_RIGHT> ListUserRight { get; set; }
        public virtual ISet<T_AD_USER_ROLE> ListUserRole { get; set; }
        public virtual ISet<T_AD_USER_HISTORY> ListUserHistory { get; set; }
        public virtual ISet<T_PS_CONFIG_HIDE_COLUMN> ConfigHideColumn { get; set; }

        private T_AD_ORGANIZE _Organize;
        public virtual T_AD_ORGANIZE Organize
        {
            get
            {
                try
                {
                    if (this._Organize != null)
                    {
                        var text = this._Organize.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Organize;
            }
            set {
                this._Organize = value;
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


        private T_MD_TITLE _Title;
        public virtual T_MD_TITLE Title
        {
            get
            {
                try
                {
                    if (this._Title != null)
                    {
                        var text = this._Title.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._Title;
            }
            set
            {
                this._Title = value;
            }
        }
        public T_AD_USER()
        {
            ListUserUserGroup = new HashSet<T_AD_USER_USER_GROUP>(new List<T_AD_USER_USER_GROUP>());
            ListUserRight = new HashSet<T_AD_USER_RIGHT>(new List<T_AD_USER_RIGHT>());
            ListUserRole = new HashSet<T_AD_USER_ROLE>(new List<T_AD_USER_ROLE>());
            ListUserHistory = new HashSet<T_AD_USER_HISTORY>(new List<T_AD_USER_HISTORY>());
            ConfigHideColumn = new HashSet<T_PS_CONFIG_HIDE_COLUMN>(new List<T_PS_CONFIG_HIDE_COLUMN>());

            Organize = new T_AD_ORGANIZE();
            Title = new T_MD_TITLE();
        }
    }
}
