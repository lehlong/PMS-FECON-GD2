using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class SystemConfigRepo : GenericRepository<T_AD_SYSTEM_CONFIG>, ISystemConfigRepo
    {
        public SystemConfigRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
