using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.CF
{
    public class ConfigDCNBRepo : GenericRepository<T_CF_DCNB>, IConfigDCNB
    {
        public ConfigDCNBRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_CF_DCNB> Search(T_CF_DCNB objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.COMPANY_CODE))
            {
                query = query.Where(x => x.COMPANY_CODE == objFilter.COMPANY_CODE);
            }
            
            query = query.OrderByDescending(x => x.COMPANY_CODE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
