using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models.Report
{
    public abstract class BaseReportModel
    {
        public BaseReportModel()
        {
            BoldRowIndexes = new List<int>();
            PercentageRowIndexes = new List<int>();
        }
        public virtual string CompanyId { get; set; }
        public virtual Guid ProjectId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IList<int> BoldRowIndexes { get; set; }
        public IList<int> PercentageRowIndexes { get; set; }
    }
}
