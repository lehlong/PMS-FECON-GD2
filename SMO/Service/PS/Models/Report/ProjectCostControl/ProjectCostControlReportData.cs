using Newtonsoft.Json;

using SMO.Core.Entities.PS;

using System;

namespace SMO.Service.PS.Models.Report.ProjectCostControl
{
    public class ProjectCostControlReportData
    {
        public string StructureCode { get; set; }
        public string StructureName { get; set; }
        public string VendorName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public decimal? MainContractVolume { get; set; }
        public decimal? MainContractPrice { get; set; }
        public decimal? MainContractTotal { get; set; }
        public decimal? CostPlanVolume { get; set; }
        public decimal? CostPlanPrice { get; set; }
        public decimal? CostPlanTotal { get; set; }
        public decimal? PlannedProfitPreTax { get; set; }
        public decimal? PlannedProfitMargin { get; set; }
        public decimal? PerformanceContractVolume { get; set; }
        public decimal? PerformanceContractTotal { get; set; }
        public decimal? PerformanceCostVolume { get; set; }
        public decimal? PerformanceCostPrice { get; set; }
        public decimal? PerformanceCostTotal { get; set; }
        public decimal? ProfitMadePreTax { get; set; }
        public decimal? ProfitMadeMargin { get; set; }
        public string Type { get; set; }
        public Guid? ParentId { get; set; }
        public Guid Id { get; set; }
        public Guid? ActivityId { get; set; }
        public Guid? WbsId { get; set; }
        public int Order { get; set; }
        public int? OrderNullRef { get; set; }
        public string ProjectCode { get; internal set; }
        public Guid LinkedBoqId { get; internal set; }

        internal void CalculateProjectValues()
        {
            PlannedProfitPreTax = MainContractTotal - CostPlanTotal;
            PlannedProfitMargin = MainContractTotal > 0 ? PlannedProfitPreTax / MainContractTotal : null;
            ProfitMadePreTax = PerformanceContractTotal - PerformanceCostTotal;
            ProfitMadeMargin = PerformanceContractTotal > 0 ? ProfitMadePreTax / PerformanceContractTotal : null;
        }
    }
}
