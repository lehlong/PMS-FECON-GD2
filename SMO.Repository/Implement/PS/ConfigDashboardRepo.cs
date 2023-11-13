using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class ConfigDashboardRepo : GenericRepository<T_PS_CONFIG_DASHBOARD>, IConfigDashboardRepo
    {
        public ConfigDashboardRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
