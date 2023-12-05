using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class DocumentWorkflowFileRepo : GenericRepository<T_PS_DOCUMENT_WORKFLOW_FILE>, IDocumentWorkflowFileRepo
    {
        public DocumentWorkflowFileRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
