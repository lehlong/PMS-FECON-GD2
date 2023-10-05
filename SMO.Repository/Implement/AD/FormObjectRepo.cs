using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class FormObjectRepo : GenericRepository<T_AD_FORM_OBJECT>, IFormObjectRepo
    {
        public FormObjectRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_AD_FORM_OBJECT> Search(T_AD_FORM_OBJECT objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.FK_FORM))
            {
                query = query.Where(x => x.FK_FORM.ToLower().Contains(objFilter.FK_FORM.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.OBJECT_CODE))
            {
                query = query.Where(x => x.OBJECT_CODE.ToLower().Contains(objFilter.OBJECT_CODE.ToLower()));
            }

            return base.Paging(query, pageSize, pageIndex, out total);
        }
    }
}
