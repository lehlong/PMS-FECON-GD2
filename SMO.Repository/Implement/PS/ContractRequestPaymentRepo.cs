
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ContractRequestPaymentRepo : GenericRepository<T_PS_CONTRACT_REQUEST_PAYMENT>, IContractRequestPaymentRepo
    {
        public ContractRequestPaymentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_PS_CONTRACT_REQUEST_PAYMENT> Search(T_PS_CONTRACT_REQUEST_PAYMENT objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (objFilter.CONTRACT_ID != Guid.Empty)
            {
                query = query.Where(x => x.CONTRACT_ID == objFilter.CONTRACT_ID);
            }

            query = query.OrderByDescending(x => x.CREATE_DATE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
