using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class CustomerOldRepo : GenericRepository<T_MD_CUSTOMER_OLD>, ICustomerOldRepo
    {
        public CustomerOldRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_CUSTOMER_OLD> Search(T_MD_CUSTOMER_OLD objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CUSTOMER_CODE))
            {
                query = query.Where(x => x.CUSTOMER_CODE.Contains(objFilter.CUSTOMER_CODE) || x.TEXT.Contains(objFilter.CUSTOMER_CODE));
            }
            if (!string.IsNullOrWhiteSpace(objFilter.COMPANY_CODE))
            {
                query = query.Where(x => x.COMPANY_CODE == objFilter.COMPANY_CODE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.PHONE))
            {
                query = query.Where(x => x.PHONE.Contains(objFilter.PHONE));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.EMAIL))
            {
                query = query.Where(x => x.EMAIL.Contains(objFilter.EMAIL));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.VAT_NUMBER))
            {
                query = query.Where(x => x.VAT_NUMBER == objFilter.VAT_NUMBER);
            }
            query = query.OrderByDescending(x => x.ACTIVE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
