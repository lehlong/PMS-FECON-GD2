using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class VendorRepo : GenericRepository<T_MD_VENDOR>, IVendorRepo
    {
        public VendorRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_VENDOR> Search(T_MD_VENDOR objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.Contains(objFilter.NAME)
                || x.CODE.Contains(objFilter.NAME)
                || x.MST.Contains(objFilter.NAME)
                || x.EMAIL.Contains(objFilter.NAME)
                || x.PHONE.Contains(objFilter.NAME)
                || x.ADDRESS.Contains(objFilter.NAME)
                || x.SHORT_NAME.Contains(objFilter.NAME)
                );
            }
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
