using System;

namespace SMO.Service.PS.Models
{
    public class RefreshTimePeriodWorkVolumeData
    {
        public Guid ProjectStructureId { get; set; }
        public decimal? PlanVolume { get; set; }
        public decimal? AccumulatedPlanVolume { get; set; }
        public decimal? AccumulateAcceptedVolume { get; set; }
        public decimal? AccumulatedVolume { get; set; }
    }
}
