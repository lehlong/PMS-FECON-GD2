using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.CF
{
    public class ConfigXBTXRepo : GenericRepository<T_CF_XBTX>, IConfigXBTX
    {
        public ConfigXBTXRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_CF_XBTX> Search(T_CF_XBTX objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CUSTOMER_CODE))
            {
                query = query.Where(x => x.CUSTOMER_CODE == objFilter.CUSTOMER_CODE);
            }
            
            query = query.OrderByDescending(x => x.CUSTOMER_CODE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
