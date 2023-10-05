using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class WbsRepo : PSRepository<T_PS_WBS>, IWbsRepo
    {
        public WbsRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
