using System;

namespace SMO.Service.PS.Models
{
    public class PlanProgressModel
    {
        public Guid Id { get; set; }
        public Guid? Parent { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectStructId { get; set; }
        public string TYPE { get; set; }
        public string CODE { get; set; }
        public string TEXT { get; set; }
        public string UNIT_NAME { get; set; }
        public decimal? CONTRACT_VALUE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime FINISH_DATE { get; set; }
    }
}
