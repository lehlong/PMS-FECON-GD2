using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;
using System.Collections.Generic;
using System.Linq;
using System;
using NHibernate.Linq;

namespace SMO.Repository.Implement.CM
{
    public class SmsRepo : GenericRepository<T_CM_SMS>, ISmsRepo
    {
        public SmsRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_CM_SMS> Search(T_CM_SMS objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.MODUL_TYPE))
            {
                query = query.Where(x => x.MODUL_TYPE == objFilter.MODUL_TYPE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.PHONE_NUMBER))
            {
                query = query.Where(x => x.PHONE_NUMBER.Contains(objFilter.PHONE_NUMBER));
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
