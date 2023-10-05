using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Service.PS.Models.Report.ProjectDetailDataCost
{
    public class ProjectDetailDataCostModel : BaseReportModel
    {
        public string Vendor { get; set; }
        public Guid? ContractId { get; set; }
        [Required(ErrorMessage = "Dự án không được để trống.")]
        public override Guid ProjectId { get; set; }

        [Required(ErrorMessage = "Công ty không được để trống.")]
        public override string CompanyId { get; set; }
    }
}
