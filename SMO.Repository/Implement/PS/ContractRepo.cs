
using NHibernate.Linq;

using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ContractRepo : GenericRepository<T_PS_CONTRACT>, IContractRepo
    {
        public ContractRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_PS_CONTRACT> Search(T_PS_CONTRACT objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                if (!objFilter.IS_SIGN_WITH_CUSTOMER)
                {
                    query = query.Where(x => x.NAME.Contains(objFilter.NAME)
                || x.CONTRACT_NUMBER.Contains(objFilter.NAME)
                || x.PO_SO_NUMBER.Contains(objFilter.NAME)
                || x.Vendor.CODE.Contains(objFilter.NAME)
                || x.Vendor.MST.Contains(objFilter.NAME)
                || x.Vendor.SHORT_NAME.Contains(objFilter.NAME)
                || x.Vendor.NAME.Contains(objFilter.NAME));
                }
                else
                {
                    query = query.Where(x => x.NAME.Contains(objFilter.NAME)
                    || x.CONTRACT_NUMBER.Contains(objFilter.NAME)
                    || x.PO_SO_NUMBER.Contains(objFilter.NAME));
                }
            }
            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }
            if (!string.IsNullOrEmpty(objFilter.CONTRACT_TYPE))
            {
                query = query.Where(x => x.CONTRACT_TYPE == objFilter.CONTRACT_TYPE);
            }
            if (!string.IsNullOrEmpty(objFilter.VENDOR_CODE))
            {
                query = query.Where(x => x.VENDOR_CODE == objFilter.VENDOR_CODE);
            }

            query = query.Where(x => x.IS_SIGN_WITH_CUSTOMER == objFilter.IS_SIGN_WITH_CUSTOMER);

            query = query.OrderByDescending(x => x.CREATE_DATE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }

        public IList<T_PS_CONTRACT> GetContractVendors(IEnumerable<Guid> projectIds, string vendor)
        {
            var query = Queryable();
            if (!string.IsNullOrEmpty(vendor))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(vendor.ToLower()) 
                || x.Vendor.MST.ToLower().Contains(vendor.ToLower()) || x.VENDOR_CODE == vendor);
            }
            query = query.Where(x => projectIds.Contains(x.PROJECT_ID) && !x.IS_SIGN_WITH_CUSTOMER);
            query = query.Fetch(x => x.Details);

            return query.ToList();
        }
        
        public IList<T_PS_CONTRACT> GetContractCustomers(IEnumerable<Guid> projectIds, string customer)
        {
            var query = Queryable();
            query = query.Where(x => projectIds.Contains(x.PROJECT_ID) && x.IS_SIGN_WITH_CUSTOMER);
            if (!string.IsNullOrEmpty(customer))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(customer.ToLower())
                || x.Vendor.MST.ToLower().Contains(customer.ToLower()) || x.CUSTOMER_CODE == customer);
            }
            query = query.Fetch(x => x.Details);

            return query.ToList();
        }
    }
}
