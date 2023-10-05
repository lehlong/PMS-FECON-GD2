using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS.Models
{
    public class PlanQuantityViewModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
        public Guid TimePeriodId { get; set; }
        public int TimePeriodOrder { get; set; }
        public Guid ProjectId { get; set; }
        public Guid ProjectStructureId { get; set; }
        public string ProjectStructureType { get; set; }
        public string ProjectStructureName { get; set; }
        public string UnitName { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? ThanhTien { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public decimal Value { get; set; }
    }
}
