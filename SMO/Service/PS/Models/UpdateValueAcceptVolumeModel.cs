using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS.Models
{
    public class UpdateValueAcceptVolumeModel
    {
        public Guid Id { get; set; }
        public Guid ProjectStructureId { get; set; }
        public Guid ProjectId { get; set; }
        public string Status { get; set; }
        public decimal? AcceptVolume { get; set; }
        public bool IsCustomer { get; set; }
        public string Notes { get; set; }
    }
}
