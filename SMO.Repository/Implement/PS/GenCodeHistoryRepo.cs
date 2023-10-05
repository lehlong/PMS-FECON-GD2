using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Implement.PS
{
    public class GenCodeHistoryRepo : GenericRepository<T_PS_GEN_CODE_HISTORY>, IGenCodeHistoryRepo
    {
        public GenCodeHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
