using System;

namespace SMO.Service.PS.Models.Report.VendorMonitoring
{
    public class VendorMonitoringReportData
    {
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string ContractCode { get; set; }
        public string StructureCode { get; set; }
        public string StructureName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public decimal? StructurePrice { get; set; }
        public decimal? ContractPrice { get; set; }
        public decimal? ContractVolume { get; set; }
        public decimal? InPeriodPlanVolume { get; set; }
        public decimal? InPeriodWorkVolume { get; set; }
        public decimal? InPeriodAcceptVolume { get; set; }
        public decimal? EndPeriodPlanVolume { get; set; }
        public decimal? EndPeriodWorkVolume { get; set; }
        public decimal? EndPeriodAcceptVolume { get; set; }

        public decimal? InPeriodPlanPrice { get; set; }
        public decimal? InPeriodWorkPrice { get; set; }
        public decimal? InPeriodAcceptPrice { get; set; }
        public decimal? EndPeriodPlanPrice { get; set; }
        public decimal? EndPeriodWorkPrice { get; set; }
        public decimal? EndPeriodAcceptPrice { get; set; }
        public decimal? InPeriodTotalWorkVolume { get; set; }
        public decimal? InPeriodTotalAcceptVolume { get; set; }
        public decimal? EndPeriodTotalWorkVolume { get; set; }
        public decimal? EndPeriodTotalAcceptVolume { get; set; }


        public bool IsSummary { get; set; }
        public double StructureOrder { get; internal set; }
        public Guid ProjectId { get; internal set; }
        public Guid? Id { get; internal set; }
        public Guid? ParentId { get; internal set; }
    }
}
