using Newtonsoft.Json;

using System;

namespace SMO.Service.PS.Models.Report.CustomerContract
{
    public class CustomerContractReportData
    {
        public string ContractCode { get; set; }
        public string StructureCode { get; set; }
        public string StructureName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public decimal? Price { get; set; }
        public decimal? ContractValue { get; set; }
        public decimal? ContractTotal { get; set; }
        public decimal? PlanVolume { get; set; }
        public decimal? PlanTotal { get; set; }
        public decimal? StartPeriodWorkVolume { get; set; }
        public decimal? StartPeriodAcceptedVolume { get; set; }
        public decimal? InPeriodPlanVolume { get; set; }
        public decimal? InPeriodTotalPlanVolume { get; set; }
        public decimal? InPeriodWorkVolume { get; set; }
        public decimal? InPeriodAcceptedVolume { get; set; }
        public decimal? EndPeriodPlanCost { get; set; }
        public bool IsSummary { get; set; }

        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
        public decimal? StartPeriodTotalWorkVolume { get; internal set; }
        public decimal? StartPeriodTotalAcceptedVolume { get; internal set; }
        public decimal? InPeriodTotalWorkVolume { get; internal set; }
        public decimal? InPeriodTotalAcceptedVolume { get; internal set; }
        public decimal? EndPeriodTotalPlanCost { get; internal set; }
        public double Order { get; internal set; }
    }
}
