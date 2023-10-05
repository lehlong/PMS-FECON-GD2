using SMO.Core.Entities.PS;

using System;

namespace SMO.Service.PS.Models
{
    public class TreeContractProjectStruct
    {
        public Guid Id { get; set; }
        public Guid? Parent { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string UnitCode { get; set; }
        public string UnitName { get; set; }
        public decimal? Volume { get; set; }
        public decimal? UnitPrice { get; set; }
        public ProjectEnum Type { get; set; }
        public bool Available { get; set; }
        public bool Enabled { get; set; }
        public string ContractName { get; set; }
        public string NguoiPhuTrach { get; set; }
        public string Status { get; set; }
        public decimal? Total { get; set; }
        public string StartDate { get; set; }
        public string FinishDate { get; set; }
        public string WbsParent { get; set; }
        public string BoqRef { get; set; }

    }
}
