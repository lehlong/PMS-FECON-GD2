using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class LanguageRepo : GenericRepository<T_AD_LANGUAGE>, ILanguageRepo
    {
        public LanguageRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_AD_LANGUAGE> Search(T_AD_LANGUAGE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.FK_CODE))
            {
                query = query.Where(x => x.FK_CODE.ToLower().Contains(objFilter.FK_CODE.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.OBJECT_TYPE))
            {
                query = query.Where(x => x.OBJECT_TYPE.ToLower().Contains(objFilter.OBJECT_TYPE.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.FORM_CODE))
            {
                query = query.Where(x => x.FORM_CODE.ToLower().Contains(objFilter.FORM_CODE.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.LANG))
            {
                query = query.Where(x => x.LANG.ToLower().Contains(objFilter.LANG.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.VALUE))
            {
                query = query.Where(x => x.VALUE.ToLower().Contains(objFilter.VALUE.ToLower()));
            }

            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
