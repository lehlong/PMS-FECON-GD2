using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.CF
{
    public class ConfigDCCHRepo : GenericRepository<T_CF_DCCH>, IConfigDCCH
    {
        public ConfigDCCHRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_CF_DCCH> Search(T_CF_DCCH objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.SALEOFFICE_CODE))
            {
                query = query.Where(x => x.SALEOFFICE_CODE == objFilter.SALEOFFICE_CODE);
            }
            
            query = query.OrderByDescending(x => x.SALEOFFICE_CODE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
