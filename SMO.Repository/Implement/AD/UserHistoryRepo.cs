using NHibernate.Criterion;
using NHibernate.Transform;
using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class UserHistoryRepo : GenericRepository<T_AD_USER_HISTORY>, IUserHistoryRepo
    {
        public UserHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_AD_USER_HISTORY> Search(T_AD_USER_HISTORY objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_AD_USER_HISTORY>();

            if (!string.IsNullOrWhiteSpace(objFilter.USER_NAME))
            {
                query = query.Where(x => x.USER_NAME == objFilter.USER_NAME);
            }
            query = query.OrderBy(x => x.LOGON_TIME).Desc;
            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
