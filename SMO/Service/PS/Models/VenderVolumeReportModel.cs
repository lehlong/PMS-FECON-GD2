using System;

namespace SMO.Service.PS.Models
{
    public class VenderVolumeReportModel
    {
        public VenderVolumeReportModel()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now;
        }
        public string VendorCode { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
