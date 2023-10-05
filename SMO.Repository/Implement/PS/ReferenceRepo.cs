using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ReferenceRepo : GenericRepository<T_PS_REFERENCE>, IReferenceRepo
    {
        public ReferenceRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_PS_REFERENCE> Search(T_PS_REFERENCE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
