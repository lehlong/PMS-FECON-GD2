using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;
using System.Collections.Generic;
using System.Linq;
using System;
using NHibernate.Linq;

namespace SMO.Repository.Implement.CM
{
    public class SmsHistoryRepo : GenericRepository<T_CM_SMS_HISTORY>, ISmsHistoryRepo
    {
        public SmsHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_CM_SMS_HISTORY> Search(T_CM_SMS_HISTORY objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.TYPE))
            {
                query = query.Where(x => x.TYPE == objFilter.TYPE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.PHONE))
            {
                query = query.Where(x => x.PHONE.Contains(objFilter.PHONE) || x.CONTENTS.Contains(objFilter.PHONE));
            }

            if (objFilter.IS_SEND)
            {
                query = query.Where(x => x.IS_SEND == true); 
            }

            if (objFilter.FROM_DATE.HasValue)
            {
                query = query.Where(x => x.CREATE_DATE >= objFilter.FROM_DATE.Value);
            }

            if (objFilter.TO_DATE.HasValue)
            {
                query = query.Where(x => x.CREATE_DATE < objFilter.TO_DATE.Value.AddDays(1));
            }

            query = query.OrderByDescending(x => x.CREATE_DATE);
            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
