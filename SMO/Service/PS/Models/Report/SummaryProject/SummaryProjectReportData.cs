namespace SMO.Service.PS.Models.Report.SummaryProject
{
    public class SummaryProjectReportData
    {
        public int Index { get; set; }
        public string ProjectName { get; set; }
        public string Customer { get; set; }
        public string SignedDate { get; set; }
        public string ContractCode { get; set; }
        public decimal ContractTotal { get; set; }
        public decimal InPeriodPlanValue { get; set; }
        public decimal InPeriodPerformanceValue { get; set; }
        public decimal InPeriodPlannedRevenue { get; set; }
        public decimal InPeriodPerformanceRevenue { get; set; }
        public decimal InPeriodPlanCosts { get; set; }
        public decimal InPeriodImplementedCosts { get; set; }
        public decimal InPeriodPlannedProceeds { get; set; }
        public decimal InPeriodProceedsMadeMoney { get; set; }
        public decimal InPeriodPlanSpentMoney { get; set; }
        public decimal InPeriodImplementedSpentMoney { get; set; }
        public decimal InPeriodVolumeWorkCosts { get; set; }
        public decimal DuringYearPerformanceValue { get; set; }
        public decimal DuringYearVolumeWorkCosts { get; set; }
        public decimal DuringYearPerformanceRevenue { get; set; }
        public decimal DuringYearImplementedCosts { get; set; }
        public decimal DuringYearProceedsMadeMoney { get; set; }
        public decimal DuringYearImplementedSpentMoney { get; set; }
        public decimal AccumulatedPerformanceValue { get; set; }
        public decimal AccumulatedPlanCosts { get; set; }
        public decimal AccumulatedPerformanceRevenue { get; set; }
        public decimal AccumulatedImplementedCosts { get; set; }
        public decimal AccumulatedProceedsMadeMoney { get; set; }
        public decimal AccumulatedImplementedSpentMoney { get; set; }
        public decimal AccumulatedPlanCost { get; set; }
        public decimal AccumulatedPlannedRevenue { get; set; }
        public decimal AccumulatedPlannedValue { get; set; }
        public decimal AccumulatedVolumeWorkCosts { get; set; }
        public decimal? TotalCost { get; set; }
        public string PhongBan { get; set; }
        public string GiamDocDuAn { get; set; }
        public string ProjectManager { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public string DonVi { get; set; }
        public string Status { get; set; }
    }
}
