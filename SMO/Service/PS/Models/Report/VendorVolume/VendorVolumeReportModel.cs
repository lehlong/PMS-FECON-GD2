using System;

namespace SMO.Service.PS.Models.Report.VendorVolume
{
    public class VendorVolumeReportModel : BaseReportModel
    {
        public string Vendor { get; set; }
        public string DepartmentId { get; set; }
        public ProjectStatus? Status { get; set; }
    }
}
