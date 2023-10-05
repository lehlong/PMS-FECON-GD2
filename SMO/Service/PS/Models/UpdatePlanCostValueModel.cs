using System;

namespace SMO.Service.PS.Models
{
    public class UpdatePlanCostValueModel
    {
        public Guid PeriodId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectStructId { get; set; }
        public decimal Value { get; set; }
        public decimal Total { get; set; }
        public bool IsCustomer { get; set; }
    }
}
