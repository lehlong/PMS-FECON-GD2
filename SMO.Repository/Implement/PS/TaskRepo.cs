using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Implement.PS
{
    public class TaskRepo : GenericRepository<T_PS_TASK>, ITaskRepo
    {
        public TaskRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public T_PS_TASK GetDetail(Guid id)
        {
            var query = NHibernateSession.QueryOver<T_PS_TASK>();
            query = query.Fetch(x => x.UserAprove).Eager;
            query = query.Fetch(x => x.UserPerformer).Eager;
            query = query.Where(x => x.ID == id);
            return query.SingleOrDefault();
        }

        public override IList<T_PS_TASK> Search(T_PS_TASK objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_PS_TASK>();

            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }
            if (objFilter.ACTIVITY_PARENT_ID != Guid.Empty)
            {
                query = query.Where(x => x.ACTIVITY_PARENT_ID == objFilter.ACTIVITY_PARENT_ID);
            }
            if (objFilter.USER_PERFORMER != Guid.Empty)
            {
                query = query.Where(x => x.USER_PERFORMER == objFilter.USER_PERFORMER);
            }
            query = query.Fetch(x => x.UserAprove).Eager;
            query = query.Fetch(x => x.UserPerformer).Eager;
            query = query.OrderBy(x => x.CREATE_DATE).Desc;
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
