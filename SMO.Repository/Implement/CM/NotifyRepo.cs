using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;
using System.Collections.Generic;
using System.Linq;
using System;
using NHibernate.Linq;

namespace SMO.Repository.Implement.CM
{
    public class NotifyRepo : GenericRepository<T_CM_NOTIFY>, INotifyRepo
    {
        public NotifyRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_CM_NOTIFY> Search(T_CM_NOTIFY objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();
            query = query.Where(x => x.USER_NAME == objFilter.USER_NAME).OrderByDescending(x => x.CREATE_DATE);
            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
