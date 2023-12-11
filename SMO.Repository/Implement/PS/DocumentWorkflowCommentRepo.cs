using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class DocumentWorkflowCommentRepo : GenericRepository<T_PS_DOCUMENT_WORKFLOW_COMMENT>, IDocumentWorkflowCommentRepo
    {
        public DocumentWorkflowCommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
