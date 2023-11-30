using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class WorkflowFileRepo : GenericRepository<T_MD_WORKFLOW_FILE>, IWorkflowFileRepo
    {
        public WorkflowFileRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
