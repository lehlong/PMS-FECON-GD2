using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.CF
{
    public class ConfigTemplateTransferRepo : GenericRepository<T_CF_TEMPLATE_TRANSFER>, IConfigTemplateTransferRepo
    {
        public ConfigTemplateTransferRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
