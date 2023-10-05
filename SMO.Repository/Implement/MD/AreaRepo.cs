using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class AreaRepo : GenericRepository<T_MD_AREA>, IAreaRepo
    {
        public AreaRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_AREA> Search(T_MD_AREA objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.Contains(objFilter.NAME) || x.CODE.Contains(objFilter.NAME));
            }
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
