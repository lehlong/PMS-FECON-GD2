using SMO.Core.Common;
using SMO.Core.Entities.MD;

using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_PROJECT : BasePSEntity
    {
        public virtual Guid ID { get; set; }
        [Required (ErrorMessage = "Tên dự án bắt buộc nhập", AllowEmptyStrings =false)]
        public virtual string NAME { get; set; }
        [Required(ErrorMessage = "Mã dự án bắt buộc nhập", AllowEmptyStrings = false)]
        public override string CODE { get; set; }
        public virtual string DESCRIPTION { get; set; }

        public virtual string STATUS { get; set; }
        public virtual string STATUS_VENDOR_PLAN_COST { get; set; }
        public virtual string STATUS_VENDOR_PLAN_PROGRESS { get; set; }
        public virtual string STATUS_VENDOR_PLAN_QUANTITY { get; set; }
        public virtual string STATUS_CUSTOMER_PLAN_COST { get; set; }
        public virtual string STATUS_CUSTOMER_PLAN_PROGRESS { get; set; }
        public virtual string STATUS_CUSTOMER_PLAN_QUANTITY { get; set; }
        public virtual string STATUS_STRUCT_COST { get; set; }
        public virtual string STATUS_STRUCT_BOQ { get; set; }
        public virtual string STATUS_STRUCT_PLAN { get; set; }
        public virtual string STATUS_SL_DT { get; set; }
        [Required (ErrorMessage = "Ngày bắt đầu bắt buộc nhập", AllowEmptyStrings =false)]
        public override DateTime START_DATE { get; set; } = DateTime.Now;
        [Required (ErrorMessage = "Ngày kết thúc bắt buộc nhập", AllowEmptyStrings =false)]
        public override DateTime FINISH_DATE { get; set; } = DateTime.Now;
        /// <summary>
        /// Period of time: week, month, ...
        /// </summary>
        [Required (ErrorMessage = "Kỳ thời gian bắt buộc nhập", AllowEmptyStrings =false)]
        public virtual string TIME_TYPE { get; set; }
        [Required(ErrorMessage = "Loại dự án bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string TYPE { get; set; }
        public virtual string PROJECT_LEVEL_CODE { get; set; }
        [Required (ErrorMessage = "Người phụ trách (SM) bắt buộc nhập", AllowEmptyStrings =false)]
        public virtual string PROJECT_OWNER { get; set; }
        [Required(ErrorMessage = "Công ty phụ trách bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string DON_VI { get; set; }
        [Required(ErrorMessage = "Phòng ban phụ trách bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string PHONG_BAN { get; set; }
        public virtual string DIA_DIEM { get; set; }
        [Required (ErrorMessage = "Khách hàng bắt buộc nhập", AllowEmptyStrings =false)]
        public virtual string CUSTOMER_CODE { get; set; }
        [Required (ErrorMessage = "Lãnh đạo phụ trách bắt buộc nhập", AllowEmptyStrings =false)]
        public virtual string GIAM_DOC_DU_AN { get; set; }
        [Required (ErrorMessage = "PM dự án bắt buộc nhập", AllowEmptyStrings =false)]
        public virtual string QUAN_TRI_DU_AN { get; set; }
        [Required(ErrorMessage = "Phụ trách cung ứng bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string PHU_TRACH_CUNG_UNG { get; set; }
        [Required(ErrorMessage = "Người quản lý hợp đồng bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string QUAN_LY_HOP_DONG { get; set; }
        public virtual string KHU_VUC { get; set; }
        [Required(ErrorMessage = "Ngày quyết toán bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual DateTime? NGAY_QUYET_TOAN { get; set; }
        [Required(ErrorMessage = "Ngày hết hạn bảo hành bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual DateTime? HAN_BAO_HANH { get; set; }
        public virtual DateTime? FINISH_DATE_ACTUAL { get; set; }
        public virtual DateTime? NGAY_TRIEN_KHAI { get; set; }
        public virtual string PLAN_METHOD { get; set; }
        public virtual string PLANT { get; set; }
        public virtual string SALE_ORG { get; set; }
        public virtual string DISTRIBUTION { get; set; }
        public virtual string FACTORY_CALENDAR { get; set; }
        public virtual string TIME_UNIT { get; set; }
        [Required(ErrorMessage = "Nhóm mua hàng bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string PUR_GROUP { get; set; }
        public virtual string FILE_NAME { get; set; }
        public virtual bool IS_CREATE_ON_SAP { get; set; }

        public virtual decimal? TOTAL_COST { get; set; }
        public virtual decimal? TOTAL_BOQ { get; set; }

        public virtual T_MD_PROJECT_TIME_PERIOD Period { get; set; }
        public virtual T_AD_USER ProjectOwner { get; set; }
        public virtual T_AD_USER GiamDocDuAn { get; set; }
        public virtual T_AD_USER QuanTriDuAn { get; set; }
        public virtual T_AD_USER PhuTrachCungUng { get; set; }
        public virtual T_AD_USER QuanLyHopDong { get; set; }

        private T_MD_PROJECT_TYPE _ProjectType;
        public virtual T_MD_PROJECT_TYPE ProjectType
        {
            get
            {
                try
                {
                    if (this._ProjectType != null)
                    {
                        var text = this._ProjectType.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._ProjectType;
            }
            set
            {
                this._ProjectType = value;
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

        private T_AD_ORGANIZE _DonVi;
        public virtual T_AD_ORGANIZE DonVi
        {
            get
            {
                try
                {
                    if (this._DonVi != null)
                    {
                        var text = this._DonVi.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._DonVi;
            }
            set
            {
                this._DonVi = value;
            }
        }

        private T_MD_AREA _KhuVuc;
        public virtual T_MD_AREA KhuVuc
        {
            get
            {
                try
                {
                    if (this._KhuVuc != null)
                    {
                        var text = this._KhuVuc.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._KhuVuc;
            }
            set
            {
                this._KhuVuc = value;
            }
        }

        private T_AD_ORGANIZE _PhongBan;
        public virtual T_AD_ORGANIZE PhongBan
        {
            get
            {
                try
                {
                    if (this._PhongBan != null)
                    {
                        var text = this._PhongBan.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._PhongBan;
            }
            set
            {
                this._PhongBan = value;
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

        private T_MD_TIME_TYPE _TimeType;
        public virtual T_MD_TIME_TYPE TimeType
        {
            get
            {
                try
                {
                    if (this._TimeType != null)
                    {
                        var text = this._TimeType.NAME;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
                return this._TimeType;
            }
            set
            {
                this._TimeType = value;
            }
        }

        protected override T_PS_PROJECT_STRUCT CastToProjectStruct()
        {
            return new T_PS_PROJECT_STRUCT
            {
                PROJECT_ID = ID,
                START_DATE = START_DATE,
                FINISH_DATE = FINISH_DATE,
                TEXT = NAME,
                CREATE_BY = CREATE_BY,
                TYPE = ProjectEnum.PROJECT.ToString(),
            };
        }
    }
}
