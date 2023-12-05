using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ProjectWorkflowStepRepo : GenericRepository<T_PS_PROJECT_WORKFLOW_STEP>, IProjectWorkflowStepRepo
    {
        public ProjectWorkflowStepRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

    }
}
