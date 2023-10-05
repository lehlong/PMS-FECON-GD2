using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models.Report.ProjectDetailDataCost
{
    public class ProjectDetailDataCost
    {
        public ProjectDetailDataCost()
        {
            DataCostPeriods = new List<ProjectDetailDataCostPeriod>();
        }
        public Guid? Id { get; set; }
        public Guid? ParentId { get; set; }
        public string ContractCode { get; set; }
        public string Type { get; set; }
        public string ProjectStructureCode { get; set; }
        public string ProjectStructureName { get; set; }
        public string VendorName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public IEnumerable<ProjectDetailDataCostPeriod> DataCostPeriods { get; set; }
        public decimal Price { get; internal set; }
        public decimal ContractPrice { get; internal set; }
    }
}

public class ProjectDetailDataCostPeriod
{
    public Guid PeriodId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime ToDate { get; set; }
    /// <summary>
    /// Khối lượng kế hoạch
    /// </summary>
    public decimal PlanVolume { get; set; }
    /// <summary>
    /// Khối lượng thực hiện
    /// </summary>
    public decimal WorkedVolume { get; set; }
    public decimal? WorkedPrice { get; set; }
    /// <summary>
    /// Khối lượng nghiệm thu
    /// </summary>
    public decimal AcceptedVolume { get; set; }
    public decimal? AcceptedPrice { get; set; }
    public int Order { get; internal set; }
}
