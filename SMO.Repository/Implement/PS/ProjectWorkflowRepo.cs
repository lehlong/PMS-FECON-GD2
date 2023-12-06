using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ProjectWorkflowRepo : GenericRepository<T_PS_PROJECT_WORKFLOW>, IProjectWorkflowRepo
    {
        public ProjectWorkflowRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_PS_PROJECT_WORKFLOW> Search(T_PS_PROJECT_WORKFLOW objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();
            if (!string.IsNullOrWhiteSpace(objFilter.PROJECT_ID.ToString()))
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }
            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.Contains(objFilter.NAME) || x.CODE.Contains(objFilter.NAME));
            }
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
