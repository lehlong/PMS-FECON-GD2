using System;

namespace SMO.Service.PS.Models
{
    public class RefreshTimePeriodModel
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string PartnerCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public bool IsCustomer { get; set; }
    }
}
