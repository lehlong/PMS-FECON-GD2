using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CF;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.CF
{
    public class StoreMaterialRepo : GenericRepository<T_CF_STORE_MATERIAL>, IStoreMaterialRepo
    {
        public StoreMaterialRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
