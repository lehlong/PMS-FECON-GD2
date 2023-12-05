using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ProjectWorkflowFileRepo : GenericRepository<T_PS_PROJECT_WORKFLOW_FILE>, IProjectWorkflowFileRepo
    {
        public ProjectWorkflowFileRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
