
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class ContractFileRepo : GenericRepository<T_PS_CONTRACT_FILE>, IContractFileRepo
    {
        public ContractFileRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
