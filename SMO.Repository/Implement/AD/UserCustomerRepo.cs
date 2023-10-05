using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class UserCustomerOldRepo : GenericRepository<T_AD_USER_CUSTOMER>, IUserCustomerOldRepo
    {
        public UserCustomerOldRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
