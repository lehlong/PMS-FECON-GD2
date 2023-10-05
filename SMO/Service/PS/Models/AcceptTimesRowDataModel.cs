using System;

namespace SMO.Service.PS.Models
{
    public class AcceptTimesRowDataModel
    {
        public Guid ProjectStructureId { get; set; }
        public decimal WorkVolume { get; set; }
        public decimal AcceptVolume { get; set; }
        public decimal Price { get; set; }
        public decimal PendingVolume { get; set; }
        public decimal AccumulateAllowedVolume { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string Notes { get; set; }
    }
}
