using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SMO.Service.PS.Models.Report.ResourceReport
{
    public class ProjectResourceModel : BaseReportModel
    {
        public string CompanyId { get; set; }
        public Guid ProjectId { get; set; }
        public string TypeResource { get; set; }
        public string Role { get; set; }
        public string ResourceOther { get; set; }
        public string Username { get; set; }
    }
}