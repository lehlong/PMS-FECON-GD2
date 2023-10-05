using SMO.Service.PS.Models.Report;
using System;

namespace SMO.Service.PS.Models
{
    public class VolumeWorkViewModel
    {
        public Guid? DbId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectStructureId { get; set; }
        public string ProjectStructureType { get; set; }
        public string ProjectStructureName { get; set; }
        public string UnitCode { get; set; }
        public decimal? ContractVolume { get; set; }
        public decimal? AcceptVolume { get; set; }
        /// <summary>
        /// Khoi luong thiet ke
        /// Gia tri luu trong bang PLAN_VOLUME_DESIGN
        /// </summary>
        public decimal? ContractEstimate { get; set; }
        /// <summary>
        /// Khoi luong ke hoach
        /// Lay theo gia tri san luong ke hoach
        /// </summary>
        public decimal? PlanVolume { get; set; }
        public decimal? AccumulatedPlanVolume { get; set; }
        public decimal? WorkVolume { get; set; }
        public decimal? Price { get; set; }
        public decimal? PercentageDone { get; set; }
        public decimal? AccumulatedVolume { get; set; }
        public decimal? TotalAccumulatedVolume { get; set; }
        public decimal? AccumulateAcceptedVolume { get; set; }
        public decimal? DesignVolume { get; set; }
        public DateTime? FinishedDate { get; set; }
        public string Notes { get; set; }
        public Guid? ReferenceFileId { get; set; }
        public Guid ContractDetailId { get; set; }
        public string ContractName { get; set; }
        public string ContractCode { get; set; }
        public string ProjectStructureCode { get; set; }
        public double Order { get; set; }
        public string WorkStatus { get; set; }
        public string WorkStatusCode { get; set; }
        public string UserUpdated { get; set; }
        public Guid? ParentId { get; internal set; }
        public string Vendor { get; set; }
        public string DepartmentId { get; set; }
        public ProjectStatus? Status { get; set; }
        public string Type { get; set; }
    }
}
