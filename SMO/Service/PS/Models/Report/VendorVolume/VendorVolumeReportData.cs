using System;

namespace SMO.Service.PS.Models.Report.VendorVolume
{
    public class VendorVolumeReportData
    {
        public Guid Id { get; set; }
        public string VendorName { get; set; }
        public string VendorCode { get; set; }
        public string ContractCode { get; set; }
        public string Type { get; set; }
        public string StructureCode { get; set; }
        public string StructureName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public decimal? TotalContractValue { get; set; }
        public decimal StartPeriodWorkVolume { get; set; }
        public decimal StartPeriodAcceptVolume { get; set; }
        public decimal InPeriodPlanVolume { get; set; }
        public decimal InPeriodWorkVolume { get; set; }
        public decimal InPeriodAcceptVolume { get; set; }
        public decimal EndPeriodPlanCost { get; set; }
        public double StructureOrder { get; internal set; }
        public Guid ProjectId { get; internal set; }
    }
}
