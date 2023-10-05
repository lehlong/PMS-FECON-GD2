namespace SMO.Service.PS.Models.Report.VendorMonitoring
{
    public class VendorMonitoringReportModel : BaseReportModel
    {
        public string Vendor { get; set; }
        public string DepartmentId { get; set; }
        public string GiamDocDuAn { get; set; }
        public string ProjectType { get; set; }
        public string ProjectLevel { get; set; }
        public ProjectStatus? Status { get; set; }
    }
}
