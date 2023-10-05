using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class ProjectProfileRepo : GenericRepository<T_MD_PROJECT_PROFILE>, IProjectProfileRepo
    {
        public ProjectProfileRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_PROJECT_PROFILE> Search(T_MD_PROJECT_PROFILE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.COMPANY_CODE))
            {
                query = query.Where(x => x.COMPANY_CODE.Contains(objFilter.COMPANY_CODE) || x.PROJECT_PROFILE.Contains(objFilter.COMPANY_CODE) || 
                x.PROJECT_TYPE.Contains(objFilter.COMPANY_CODE));
            }
            query = query.OrderBy(x => x.COMPANY_CODE).ThenBy(x => x.PROJECT_TYPE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
