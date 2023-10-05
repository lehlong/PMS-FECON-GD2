using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS.Models
{
    public class UpdateActiveVolumeWorkModel
    {
        public UpdateActiveVolumeWorkModel()
        {
            ProjectStructureIds = new List<Guid>();
        }
        public IList<Guid> ProjectStructureIds { get; set; }
        public Guid TimePeriodId { get; set; }
        public Guid ProjectId { get; set; }
        public bool IsCustomer { get; set; }
        public bool Active { get; set; }
    }
}
