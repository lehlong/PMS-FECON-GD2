using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS.Models.Report.CustomerContract
{
    public class CustomerContractReportModel : BaseReportModel
    {
        public string DepartmentId { get; set; }
        public string GiamDocDuAn { get; set; }
        public string ProjectType { get; set; }
        public string ProjectLevel { get; set; }
        public string Customer { get; set; }
        public ProjectStatus? Status { get; set; }
    }
}
