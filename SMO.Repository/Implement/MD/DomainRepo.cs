using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class DomainRepo : GenericRepository<T_MD_DOMAIN>, IDomainRepo
    {
        public DomainRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_DOMAIN> Search(T_MD_DOMAIN objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(objFilter.CODE.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(objFilter.NAME.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.NOTE))
            {
                query = query.Where(x => x.NOTE.ToLower().Contains(objFilter.NOTE.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.DATA_TYPE))
            {
                query = query.Where(x => x.DATA_TYPE.ToLower() == objFilter.DATA_TYPE.ToLower());
            }

            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
