using System;
using System.Collections.Generic;

namespace SMO.Service.PS.Models
{
    public class TreeContractModel
    {
        public TreeContractModel()
        {
            TreeContracts = new List<TreeContractProjectStruct>();
        }
        public Guid CONTRACT_ID { get; set; }
        public IList<TreeContractProjectStruct> TreeContracts { get; set; }
    }
}
