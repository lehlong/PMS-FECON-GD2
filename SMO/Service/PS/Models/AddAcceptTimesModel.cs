
using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class AddAcceptTimesModel
    {
        public bool IsCustomer { get; set; }
        public Guid ProjectId { get; set; }
        public string PartnerCode { get; set; }
    }
}
