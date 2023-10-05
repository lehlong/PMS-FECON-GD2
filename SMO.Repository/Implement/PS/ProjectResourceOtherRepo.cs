
using NHibernate.Linq;

using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ProjectResourceOtherRepo : GenericRepository<T_PS_RESOURCE_OTHER>, IProjectResourceOtherRepo
    {
        public ProjectResourceOtherRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_PS_RESOURCE_OTHER> Search(T_PS_RESOURCE_OTHER objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }

            if (!string.IsNullOrEmpty(objFilter.FULL_NAME))
            {
                query = query.Where(x => x.FULL_NAME.ToLower().Contains(objFilter.FULL_NAME.ToLower())
                                    || x.VENDOR_CODE.ToLower().Contains(objFilter.FULL_NAME.ToLower())
                                    || x.CMT.ToLower().Contains(objFilter.FULL_NAME.ToLower())
                                    || x.PHONE.ToLower().Contains(objFilter.FULL_NAME.ToLower())
                                    || x.EMAIL.ToLower().Contains(objFilter.FULL_NAME.ToLower())
                                    || x.VAI_TRO.ToLower().Contains(objFilter.FULL_NAME.ToLower()));
            }

            if (objFilter.FILTER_FROM_DATE.HasValue)
            {
                query = query.Where(x => x.TO_DATE >= objFilter.FILTER_FROM_DATE.Value || x.FROM_DATE >= objFilter.FILTER_FROM_DATE.Value);
            }

            if (objFilter.FILTER_TO_DATE.HasValue)
            {
                query = query.Where(x => x.TO_DATE < objFilter.FILTER_TO_DATE.Value.AddDays(1) || x.FROM_DATE < objFilter.FILTER_TO_DATE.Value.AddDays(1));
            }

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }

        //public IList<T_AD_USER> SearchUser(T_AD_USER objFilter, int pageSize, int pageIndex, out int total)
        //{
        //    var query = NHibernateSession.Query<T_AD_USER>();

        //    if (!string.IsNullOrEmpty(objFilter.USER_NAME))
        //    {
        //        query = query.Where(x => x.USER_NAME.Contains(objFilter.USER_NAME) || x.FULL_NAME.Contains(objFilter.USER_NAME));
        //    }

        //    if (!string.IsNullOrEmpty(objFilter.COMPANY_ID))
        //    {
        //        query = query.Where(x => x.COMPANY_ID == objFilter.COMPANY_ID);
        //    }

        //    if (!string.IsNullOrEmpty(objFilter.VENDOR_CODE))
        //    {
        //        query = query.Where(x => x.VENDOR_CODE == objFilter.VENDOR_CODE);
        //    }

        //    if (!string.IsNullOrEmpty(objFilter.USER_TYPE))
        //    {
        //        query = query.Where(x => x.USER_TYPE == objFilter.USER_TYPE);
        //    }

        //    int startIndex = pageSize * (pageIndex - 1);
        //    var rowCount = query.ToFutureValue(x => x.Count());
        //    List<T_AD_USER> lstT = query.Skip(startIndex).Take(pageSize).ToFuture().ToList();
        //    total = rowCount.Value;
        //    return lstT;
        //}
    }
}
