using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class PoRepo : GenericRepository<T_MD_PO>, IPoRepo
    {
        public PoRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_PO> Search(T_MD_PO objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.PO_NUMBER))
            {
                query = query.Where(x => x.PO_NUMBER.Contains(objFilter.PO_NUMBER) || x.PO_TYPE == objFilter.PO_NUMBER || x.PO_ORG == objFilter.PO_NUMBER);
            }
            
            query = query.OrderByDescending(x => x.PO_LOCK).ThenBy(x => x.PO_ORG).ThenBy(x => x.PO_NUMBER).ThenBy(x => x.PO_ITEM);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
