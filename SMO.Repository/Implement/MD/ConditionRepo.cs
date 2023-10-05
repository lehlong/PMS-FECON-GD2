using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class ConditionRepo : GenericRepository<T_MD_CONDITION>, IConditionRepo
    {
        public ConditionRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_CONDITION> Search(T_MD_CONDITION objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Contains(objFilter.CODE) || x.NAME.Contains(objFilter.CODE));
            }
            
            query = query.OrderByDescending(x => x.ACTIVE).ThenBy(x => x.NAME);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
