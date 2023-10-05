
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;

namespace SMO.Repository.Implement.PS
{
    public class ContractDetailRepo : GenericRepository<T_PS_CONTRACT_DETAIL>, IContractDetailRepo
    {
        public ContractDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public IList<T_PS_CONTRACT_DETAIL> GetContractDetailsOfContract(Guid contractId)
        {
            var query = NHibernateSession.QueryOver<T_PS_CONTRACT_DETAIL>();
            query = query.Where(x => x.CONTRACT_ID == contractId);
            query = query.Fetch(x => x.Struct).Eager;
            return query.List();
        }
    }
}
