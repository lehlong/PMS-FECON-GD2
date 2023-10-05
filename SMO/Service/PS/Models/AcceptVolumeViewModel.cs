using System;

namespace SMO.Service.PS.Models
{
    public class AcceptVolumeViewModel
    {
        public Guid? Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ParentId { get; internal set; }
        public Guid ProjectStructureId { get; set; }
        public Guid ContractDetailId { get; set; }
        public Guid? ReferenceFileId { get; set; }
        public string ProjectStructureType { get; set; }
        public string ProjectStructureCode { get; set; }
        public string ProjectStructureName { get; set; }
        public string UnitCode { get; set; }
        public string ContractName { get; set; }
        public string ContractCode { get; set; }
        public decimal? ContractVolume { get; set; }
        /// <summary>
        /// KL Kế hoạch
        /// </summary>
        public decimal? PlanVolume { get; set; }
        /// <summary>
        /// LK Kế hoạch
        /// </summary>
        public decimal? AccumulatedPlanVolume { get; set; }
        /// <summary>
        /// LK thực hiện
        /// </summary>
        public decimal? AccumulatedVolume { get; set; }
        /// <summary>
        /// LK xác nhận
        /// </summary>
        public decimal? PendingVolume { get; set; }
        /// <summary>
        /// KL dở dang
        /// </summary>
        public decimal? AccumulateAcceptedVolume { get; set; }
        /// <summary>
        /// KL nghiệm thu
        /// </summary>
        public decimal? AcceptVolume { get; set; }
        public decimal? Price { get; set; }
        /// <summary>
        /// LK nghiệm thu
        /// </summary>
        public decimal? AccumulateAllowedVolume { get; set; }
        public decimal? TotalAccumulateAllowedVolume { get; set; }
        public string Notes { get; set; }
        public double Order { get; set; }
        public DateTime FinishedDate { get; set; }
        public string WorkStatus { get; set; }
        public string UserUpdated { get; set; }
    }
}
