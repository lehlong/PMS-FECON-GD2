using System;

namespace SMO.Service.PS.Models
{
    public class UpdatePlanDesignValueModel
    {
        public Guid ProjectId { get; set; }
        public Guid ProjectStructId { get; set; }
        public decimal Value { get; set; }
        public bool IsCustomer { get; set; }
    }
}
