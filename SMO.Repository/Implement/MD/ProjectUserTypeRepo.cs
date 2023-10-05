using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class ProjectUserTypeRepo : GenericRepository<T_MD_PROJECT_USER_TYPE>, IProjectUserTypeRepo
    {
        public ProjectUserTypeRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_PROJECT_USER_TYPE> Search(T_MD_PROJECT_USER_TYPE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(objFilter.NAME.ToLower()) || x.CODE.ToLower().Contains(objFilter.NAME.ToLower()));
            }
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
