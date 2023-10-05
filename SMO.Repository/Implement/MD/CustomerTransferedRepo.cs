using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class CustomerTransferedRepo : GenericRepository<T_MD_CUSTOMER_OLD_TRANSFERED>, ICustomerTransferedRepo
    {
        public CustomerTransferedRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
