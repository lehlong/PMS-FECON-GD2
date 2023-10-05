using System;

namespace SMO.Service.PS.Models
{
    public class UpdateValueModel
    {
        public Guid PeriodId { get; set; }
        public Guid ProjectId { get; set; }
        public string CriteriaCode { get; set; }
        public decimal Value { get; set; }
    }
}
