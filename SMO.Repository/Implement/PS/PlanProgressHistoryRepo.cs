using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class PlanProgressHistoryRepo : GenericRepository<T_PS_PLAN_PROGRESS_HISTORY>, IPlanProgressHistoryRepo
    {
        public PlanProgressHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
