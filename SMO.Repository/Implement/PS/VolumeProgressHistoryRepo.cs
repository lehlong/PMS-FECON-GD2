using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class VolumeProgressHistoryRepo : GenericRepository<T_PS_VOLUME_PROGRESS_HISTORY>, IVolumeProgressHistoryRepo
    {
        public VolumeProgressHistoryRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
