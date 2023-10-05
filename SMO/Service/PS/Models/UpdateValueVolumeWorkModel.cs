using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS.Models
{
    public class UpdateValueVolumeWorkModel
    {
        public Guid ProjectStructureId { get; set; }
        public Guid TimePeriodId { get; set; }
        public Guid ProjectId { get; set; }
        public string Status { get; set; }
        public bool IsCustomer { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string Notes { get; set; }
        public decimal WorkVolume { get; set; }
        public decimal? AcceptVolume { get; set; }
    }
}
