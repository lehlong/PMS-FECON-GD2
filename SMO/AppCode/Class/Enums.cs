namespace SMO
{
    public enum WorkFlowType
    {
        Process,
        Activity,
        Level,
        Sender,
        Receiver,
        Com
    }

    public enum ActivityType
    {
        Standard,
        Approve
    }

    public enum ApproveType
    {
        Approve,
        Reject
    }

    public enum Domain
    {
        LANG,
        OBJECT_TYPE,
        ORGANIZE_TYPE,
        CONFIG_TYPE,
        USER_TYPE
    }

    public enum GranttType
    {
        TASK,       // a regular task (default value).
        PROJECT,    // a task that starts, when its earliest child task starts, and ends, when its latest child ends. The start_date, end_date, duration properties are ignored for such tasks.
        MILESTONE,  // a zero-duration task that is used to mark out important dates of the project. The duration, progress, end_date properties are ignored for such tasks.
    }
    public enum GranttLinkType
    {
        FINISH_TO_START = 0,
        START_TO_START = 1,
        FINISH_TO_FINISH = 2,
        START_TO_FINISH = 3
    }

    public enum ProjectTimeTypeEnum
    {
        [EnumName("Tuần")]
        [EnumValue("01")]
        WEEK,
        [EnumValue("02")]
        [EnumName("Tháng")]
        MONTH,
    }

    public enum ProjectPartnerType
    {
        VENDOR,
        CUSTOMER,
        SL_DT,
        STRUCTURE
    }

    public enum ProjectPlanType
    {
        COST,
        PROGRESS,
        QUANTITY
    }

    public enum ProjectStatus
    {
        [EnumValue("01")]
        [EnumName("Khởi tạo")]
        KHOI_TAO,
        [EnumValue("02")]
        [EnumName("Lập kế hoạch")]
        LAP_KE_HOACH,
        [EnumValue("03")]
        [EnumName("Đang thực hiện")]
        BAT_DAU,
        [EnumValue("04")]
        [EnumName("Đóng dự án")]
        DONG_DU_AN,
        [EnumValue("05")]
        [EnumName("Hoàn thành")]
        HOAN_THANH
    }

    public enum ProjectStructStatus
    {
        [EnumValue("01")]
        [EnumName("Chưa bắt đầu")]
        KHOI_TAO,
        [EnumValue("02")]
        [EnumName("Đang thực hiện")]
        DANG_THUC_HIEN,
        [EnumValue("03")]
        [EnumName("Tạm dừng")]
        TAM_DUNG,
        [EnumValue("04")]
        [EnumName("Hoàn thành")]
        HOAN_THANH
    }

    public enum ProjectPlanStatus
    {
        [EnumValue("01")]
        CHUA_TRINH_DUYET,
        [EnumValue("02")]
        CHO_PHE_DUYET,
        [EnumValue("03")]
        TU_CHOI,
        [EnumValue("04")]
        PHE_DUYET,
        [EnumValue("05")]
        HUY_PHE_DUYET,
    }

    public enum ProjectVolume
    {
        [EnumValue("01")]
        [EnumName("Khởi tạo")]
        KHOI_TAO,
        [EnumName("Chờ xác nhận")]
        [EnumValue("02")]
        CHO_XAC_NHAN,
        [EnumName("Không xác nhận")]
        [EnumValue("03")]
        KHONG_XAC_NHAN,
        [EnumName("Xác nhận")]
        [EnumValue("04")]
        XAC_NHAN,
        [EnumName("Hủy xác nhận")]
        [EnumValue("05")]
        HUY_XAC_NHAN,
    }

    public enum ProjectWorkVolumeStatus
    {
        [EnumValue("01")]
        [EnumName("Khởi tạo")]
        KHOI_TAO,
        [EnumName("Chờ xác nhận")]
        [EnumValue("02")]
        CHO_XAC_NHAN,
        [EnumName("Không xác nhận")]
        [EnumValue("03")]
        KHONG_XAC_NHAN,
        [EnumName("Đã xác nhận")]
        [EnumValue("04")]
        XAC_NHAN,
        [EnumName("Đã phê duyệt")]
        [EnumValue("05")]
        PHE_DUYET,
        [EnumName("Từ chối")]
        [EnumValue("06")]
        TU_CHOI,
    }

    public enum ProjectWorkVolumeActiveStatus
    {
        [EnumValue("STARTED")]
        [EnumName("Đã bắt đầu")]
        STARTED,
        [EnumValue("STOPPED")]
        [EnumName("Đã tạm dừng")]
        STOPPED
    }

    public enum DocumentWorkflowStatus
    {
        [EnumValue("01")]
        [EnumName("Khởi tạo")]
        KHOI_TAO,
        [EnumName("Chờ phê duyệt")]
        [EnumValue("02")]
        CHO_PHE_DUYET,
        [EnumName("Đã phê duyệt")]
        [EnumValue("03")]
        DA_PHE_DUYET,
        [EnumName("Từ chối")]
        [EnumValue("04")]
        TU_CHOI,
    }
    public enum ProjectStructureType
    {
        COST,
        BOQ
    }
    public enum ProjectWorkVolumeAction
    {
        [EnumValue("01")]
        [EnumName("Tạo mới")]
        TAO_MOI,
        [EnumName("Gửi")]
        [EnumValue("02")]
        GUI,
        [EnumName("Không xác nhận")]
        [EnumValue("03")]
        KHONG_XAC_NHAN,
        [EnumName("Xác nhận")]
        [EnumValue("04")]
        XAC_NHAN,
        [EnumName("Phê duyệt")]
        [EnumValue("05")]
        PHE_DUYET,
        [EnumName("Từ chối")]
        [EnumValue("06")]
        TU_CHOI,
        [EnumName("Hủy phê duyệt")]
        [EnumValue("07")]
        HUY_PHE_DUYET
    }
    
    public enum ProjectStructureProgressAction
    {
        [EnumValue("01")]
        [EnumName("Tạo mới")]
        TAO_MOI,
        [EnumName("Gửi")]
        [EnumValue("02")]
        GUI,
        [EnumName("Phê duyệt")]
        [EnumValue("03")]
        PHE_DUYET,
        [EnumName("Từ chối")]
        [EnumValue("04")]
        TU_CHOI,
        [EnumName("Hủy phê duyệt")]
        [EnumValue("05")]
        HUY_PHE_DUYET,
        [EnumName("Cập nhật thông tin")]
        [EnumValue("06")]
        CAP_NHA_THONG_TIN
    }

    public enum ProjectStructureProgressStatus
    {
        [EnumValue("01")]
        [EnumName("Chưa trình duyệt")]
        KHOI_TAO,
        [EnumName("Chờ phê duyệt")]
        [EnumValue("02")]
        CHO_PHE_DUYET,
        [EnumName("Đã phê duyệt")]
        [EnumValue("05")]
        PHE_DUYET,
        [EnumName("Từ chối")]
        [EnumValue("06")]
        TU_CHOI,
    }

    public enum ProjectCriteria
    {
        [EnumValue("01")]
        [EnumName("Giá trị sản lượng")]
        GIA_TRI_SAN_LUONG,
        [EnumValue("02")]
        [EnumName("Doanh thu")]
        DOANH_THU,
        [EnumValue("03")]
        [EnumName("Tiền thu")]
        TIEN_THU,
        [EnumValue("04")]
        [EnumName("Kế hoạch chi phí")]
        KE_HOACH_CHI_PHI,
        [EnumValue("05")]
        [EnumName("Tiền chi")]
        TIEN_CHI,
    }
    public enum ConfigHideColumn
    {
        [EnumValue("CustomerContractReport")]
        BAO_CAO_THUC_HIEN_HOP_DONG_KHACH_HANG,
        [EnumValue("ProjectCostControlReport")]
        BAO_CAO_KIEM_SOAT_CHI_PHI_DU_AN,
        [EnumValue("ProjectDetailDataBoqReport")]
        XUAT_DU_LIEU_CHI_TIET_DU_AN_BOQ,
        [EnumValue("ProjectDetailDataCostReport")]
        XUAT_DU_LIEU_CHI_TIET_CHI_PHI_DU_AN,
        [EnumValue("ProjectResourceReport")]
        BAO_CAO_DANH_SACH_NHAN_SU_DU_AN,
        [EnumValue("SummaryProjectReport")]
        BAO_CAO_TONG_HOP_DU_AN,
        [EnumValue("VendorMonitoringReport")]
        BAO_CAO_THEO_DOI_THAU_PHU,
        [EnumValue("VendorVolumeReport")]
        BAO_CAO_KHOI_LUONG_THAU_PHU,
        [EnumValue("EditPlanCostCustomer")]
        KE_HOACH_SAN_LUONG,
        [EnumValue("EditPlanCostVendor")]
        KE_HOACH_CHI_PHI,
        [EnumValue("EditDtdt")]
        KE_HOACH_DOANH_THU_DONG_TIEN,
        [EnumValue("EditVolumeWorkCustomer")]
        THUC_HIEN_KHACH_HANG,
        [EnumValue("EditVolumeWorkVendor")]
        THUC_HIEN_THAU_PHU,
        [EnumValue("EditAcceptVolumeCustomer")]
        NGHIEM_THU_KHACH_HANG,
        [EnumValue("EditAcceptVolumeVendor")]
        NGHIEM_THU_THAU_PHU,

    }
}