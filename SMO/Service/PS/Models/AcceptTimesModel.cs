using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class AcceptTimesModel
    {
        public AcceptTimesModel()
        {
            Details = new List<AcceptTimesRowDataModel>();
        }
        public Guid Id { get; set; }
        public string PartnerCode { get; set; }
        public Guid ProjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public bool IsCustomer { get; set; }

        public IList<AcceptTimesRowDataModel> Details { get; set; }
    }
}
