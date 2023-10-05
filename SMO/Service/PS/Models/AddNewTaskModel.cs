using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class AddNewTaskModel
    {
        public Guid ProjectId { get; set; }
        public Guid ContractId { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Unit { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public string Parent { get; set; }
        public string ParentBoq { get; set; }
        public string PeopleResponsibility { get; set; }

    }
}
