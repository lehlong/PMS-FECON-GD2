using System;

namespace SMO.Models
{
    public class UpdateProjectPlanStatusModel
    {
        public Guid ProjectId { get; set; }
        public ProjectPartnerType Type { get; set; }
        public ProjectPlanType SubType { get; set; }
        public string Note { get; set; }
    }
}
