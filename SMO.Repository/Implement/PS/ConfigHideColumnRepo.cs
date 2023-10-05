using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class ConfigHideColumnRepo : GenericRepository<T_PS_CONFIG_HIDE_COLUMN>, IConfigHideColumnRepo
    {
        public ConfigHideColumnRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
