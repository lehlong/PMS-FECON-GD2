using NHibernate.Linq;
using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class DischardRepo : GenericRepository<T_MD_DISCHARD>, IDischardRepo
    {
        public DischardRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_DISCHARD> Search(T_MD_DISCHARD objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.Contains(objFilter.CODE) || x.TEXT.Contains(objFilter.CODE));
            }
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
