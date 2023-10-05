
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class ProgressHistoryRepo : GenericRepository<T_PS_PROGRESS_HISTORY>, IProgressHistoryRepo
    {
        public ProgressHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
