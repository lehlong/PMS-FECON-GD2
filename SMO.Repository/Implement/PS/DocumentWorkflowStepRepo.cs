﻿using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class DocumentWorkflowStepRepo : GenericRepository<T_PS_DOCUMENT_WORKFLOW_STEP>, IDocumentWorkflowStepRepo
    {
        public DocumentWorkflowStepRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

    }
}

