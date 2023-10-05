using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.CF
{
    public class ConfigTemplateNotifySumRepo : GenericRepository<T_CF_TEMPLATE_NOTIFY_SUM>, IConfigTemplateNotifySumRepo
    {
        public ConfigTemplateNotifySumRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
