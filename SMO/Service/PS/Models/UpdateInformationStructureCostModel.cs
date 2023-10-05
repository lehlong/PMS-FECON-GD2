using System;

namespace SMO.Service.PS.Models
{
    public class UpdateInformationStructureCostModel
    {
        public Guid ProjectStructId { get; set; }
        public Guid? ReferenceBoqId { get; set; }
    }
}
