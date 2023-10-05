using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class UpdateStatusVolumeWorkModel
    {
        public UpdateStatusVolumeWorkModel()
        {
            Ids = new List<Guid>();
        }
        public IList<Guid> Ids { get; set; }
        public Guid TimePeriodId { get; set; }
        public Guid ProjectId { get; set; }
        public string Action { get; set; }
        public bool IsCustomer { get; set; }
    }
}
