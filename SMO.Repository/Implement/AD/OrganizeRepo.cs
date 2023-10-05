using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class OrganizeRepo : GenericRepository<T_AD_ORGANIZE>, IOrganizeRepo
    {
        public OrganizeRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_AD_ORGANIZE> Search(T_AD_ORGANIZE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(objFilter.NAME.ToLower())
                                    || x.COMPANY_CODE.ToLower().Contains(objFilter.NAME.ToLower())
                                    || x.COST_CENTER_CODE.ToLower().Contains(objFilter.NAME.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.COMPANY_CODE))
            {
                query = query.Where(x => x.COMPANY_CODE == objFilter.COMPANY_CODE);
            }

            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
