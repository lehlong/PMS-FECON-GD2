using SMO.AppCode.GanttChart;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMO.Service.PS
{
    public class ProjectStructVersionService : GenericService<T_PS_PROJECT_STRUCT_VERSION, ProjectStructVersionRepo>
    {
        public override void Search()
        {
            NumerRecordPerPage = int.MaxValue;
            base.Search();
        }
        public ProjectStructureType Type { get; set; }

        internal void InitContractCode(List<TaskDto> taskDtos, bool isCostStructure)
        {
            var contracts = GetAllContracts(isCostStructure);
            foreach (var contract in contracts)
            {
                foreach (var contractDetail in contract.Details)
                {
                    var structure = taskDtos.FirstOrDefault(x => x.Id == contractDetail.PROJECT_STRUCT_ID);
                    if (structure != null)
                    {
                        structure.ContractCode = contract.CONTRACT_NUMBER;
                        structure.VendorName = !contract.IS_SIGN_WITH_CUSTOMER ? contract.Vendor.NAME : string.Empty;
                    }
                }
            }
        }
        private IList<T_PS_CONTRACT> GetAllContracts(bool isCostStructure)
        {
            return UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID && x.IS_SIGN_WITH_CUSTOMER != isCostStructure)
                .ToList();
        }
    }
}