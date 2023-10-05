using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class PlanCostRepo : GenericRepository<T_PS_PLAN_COST>, IPlanCostRepo
    {
        public PlanCostRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_PS_PLAN_COST> Search(T_PS_PLAN_COST objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }
            query = query.Where(x => x.IS_CUSTOMER == objFilter.IS_CUSTOMER);

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
