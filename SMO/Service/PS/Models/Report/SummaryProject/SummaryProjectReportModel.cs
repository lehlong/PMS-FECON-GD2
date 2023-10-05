namespace SMO.Service.PS.Models.Report.SummaryProject
{
    public class SummaryProjectReportModel : BaseReportModel
    {
        public string DepartmentId { get; set; }
        public string GiamDocDuAn { get; set; }
        public string ProjectType { get; set; }
        public string ProjectLevel { get; set; }
        public string Customer { get; set; }
        public ProjectStatus? Status { get; set; }
    }
}
