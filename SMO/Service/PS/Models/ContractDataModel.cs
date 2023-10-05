using System;

namespace SMO.Service.PS.Models
{
    public class ContractDataModel
    {
        public Guid ProjectStructId { get; set; }
        public string UnitCode { get; set; }
        public string Status { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? Volume { get; set; }
        public string WbsParent { get; set; }
        public string BoqRef { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public bool IsEnable { get; set; }
    }
}
