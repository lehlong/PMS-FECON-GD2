using System;

namespace SMO.Service.PS.Models
{
    public class UpdateProjectStructureStatusModel
    {
        public Guid ProjectId { get; set; }
        public ProjectStructureProgressAction Action { get; set; }
        public string Note { get; set; }
    }
}
