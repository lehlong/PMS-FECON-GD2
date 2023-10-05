using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class UpdateContractModel
    {
        public Guid ProjectId { get; set; }
        public Guid ContractId { get; set; }
        public ContractDataModel Data { get; set; }
    }
}
