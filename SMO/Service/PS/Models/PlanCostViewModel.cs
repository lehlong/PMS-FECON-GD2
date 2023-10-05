using System;

namespace SMO.Service.PS.Models
{
    public class PlanCostViewModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid TimePeriodId { get; set; }
        public int TimePeriodOrder { get; set; }
        public double Order { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectStructureId { get; set; }
        public string GenCode { get; set; }
        public string ProjectStructureType { get; set; }
        public string ProjectStructureName { get; set; }
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? PlanVolume { get; set; }
        public decimal? ThanhTien { get; set; }
        public decimal? PeriodTotal { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public int Duration { get; set; }
        public decimal Value { get; set; }
    }
}
