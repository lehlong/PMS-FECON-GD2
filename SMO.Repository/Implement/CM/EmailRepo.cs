using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;
using System.Collections.Generic;
using System.Linq;
using System;
using NHibernate.Linq;

namespace SMO.Repository.Implement.CM
{
    public class EmailRepo : GenericRepository<T_CM_EMAIL>, IEmailRepo
    {
        public EmailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_CM_EMAIL> Search(T_CM_EMAIL objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.EMAIL))
            {
                query = query.Where(x => x.EMAIL.Contains(objFilter.EMAIL));
            }

            if (objFilter.IS_SEND)
            {
                query = query.Where(x => x.IS_SEND == false);
            }

            query = query.OrderByDescending(x => x.CREATE_DATE);
            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
