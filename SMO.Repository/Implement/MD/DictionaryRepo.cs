using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class DictionaryRepo : GenericRepository<T_MD_DICTIONARY>, IDictionaryRepo
    {
        public DictionaryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_DICTIONARY> Search(T_MD_DICTIONARY objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.FK_DOMAIN))
            {
                query = query.Where(x => x.FK_DOMAIN.ToLower().Contains(objFilter.FK_DOMAIN.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(objFilter.CODE.ToLower()));
            }

            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
