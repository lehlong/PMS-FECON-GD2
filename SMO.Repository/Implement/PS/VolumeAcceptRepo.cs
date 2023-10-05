
using NHibernate.Criterion;

using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class VolumeAcceptRepo : GenericRepository<T_PS_VOLUME_ACCEPT>, IVolumeAcceptRepo
    {
        public VolumeAcceptRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_PS_VOLUME_ACCEPT> Search(T_PS_VOLUME_ACCEPT objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();
            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }
            if (!string.IsNullOrEmpty(objFilter.VENDOR_CODE))
            {
                query = query.Where(x => x.VENDOR_CODE == objFilter.VENDOR_CODE);
            }
            if (objFilter.FILTER_FROM_DATE.HasValue)
            {
                query = query.Where(x => x.TO_DATE >= objFilter.FILTER_FROM_DATE || x.FROM_DATE >= objFilter.FILTER_FROM_DATE);
            }

            if (objFilter.FILTER_TO_DATE.HasValue)
            {
                query = query.Where(x => x.TO_DATE < objFilter.FILTER_TO_DATE || x.FROM_DATE < objFilter.FILTER_TO_DATE);
            }
            query = query.Where(x => x.IS_CUSTOMER == objFilter.IS_CUSTOMER);
            query = query.OrderByDescending(x => x.UPDATE_TIMES);

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
        public IList<T_PS_VOLUME_ACCEPT> GetWithDetails(IEnumerable<Guid> projectIds, string status, bool? isCustomer)
        {
            var query = NHibernateSession.QueryOver<T_PS_VOLUME_ACCEPT>();
            query = query.Fetch(x => x.Details).Eager;
            query = query.Where(x => x.PROJECT_ID.IsIn(projectIds.ToArray()) && x.STATUS == status);
            if (isCustomer.HasValue)
            {
                query = query.Where(x => x.IS_CUSTOMER == isCustomer);
            }
            return query.List();
        }
    }
}
