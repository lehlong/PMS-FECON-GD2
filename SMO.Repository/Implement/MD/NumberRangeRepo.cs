using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class NumberRangeRepo : GenericRepository<T_MD_NUMBER_RANGE>, INumberRangeRepo
    {
        public NumberRangeRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_NUMBER_RANGE> Search(T_MD_NUMBER_RANGE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CHARACTER))
            {
                query = query.Where(x => x.CHARACTER.Contains(objFilter.CHARACTER));
            }
            query = query.OrderBy(x => x.CHARACTER).ThenBy(x => x.CURRENT_NUMBER);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
