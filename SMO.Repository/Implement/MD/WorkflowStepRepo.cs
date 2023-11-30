using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class WorkflowStepRepo : GenericRepository<T_MD_WORKFLOW_STEP>, IWorkflowStepRepo
    {
        public WorkflowStepRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

    }
}
