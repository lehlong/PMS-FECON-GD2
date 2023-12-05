using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class DocumentWorkflowRepo : GenericRepository<T_PS_DOCUMENT_WORKFLOW>, IDocumentWorkflowRepo
    {
        public DocumentWorkflowRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_PS_DOCUMENT_WORKFLOW> Search(T_PS_DOCUMENT_WORKFLOW objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.Contains(objFilter.NAME) || x.CODE.Contains(objFilter.NAME));
            }
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
