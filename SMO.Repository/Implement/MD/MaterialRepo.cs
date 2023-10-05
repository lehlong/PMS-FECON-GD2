using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class MaterialRepo : GenericRepository<T_MD_MATERIAL>, IMaterialRepo
    {
        public MaterialRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_MATERIAL> Search(T_MD_MATERIAL objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(objFilter.CODE.ToLower()) ||
                    x.TEXT.Contains(objFilter.CODE) ||
                    x.TYPE.Contains(objFilter.CODE));
            }
            if (!string.IsNullOrWhiteSpace(objFilter.TYPE))
            {
                query = query.Where(x => x.TYPE == objFilter.TYPE);
            }
            query = query.OrderByDescending(x => x.ACTIVE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
