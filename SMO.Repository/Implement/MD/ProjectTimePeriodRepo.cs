using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

namespace SMO.Repository.Implement.MD
{
    public class ProjectTimePeriodRepo : GenericRepository<T_MD_PROJECT_TIME_PERIOD>, IProjectTimePeriodRepo
    {
        public ProjectTimePeriodRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
