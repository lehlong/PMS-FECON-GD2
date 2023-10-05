using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class MessageRepo : GenericRepository<T_AD_MESSAGE>, IMessageRepo
    {
        public MessageRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_AD_MESSAGE> Search(T_AD_MESSAGE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(objFilter.CODE.ToLower()) || x.MESSAGE.ToLower().Contains(objFilter.CODE.ToLower()));
            }
            
            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
