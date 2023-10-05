using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class VolumeWorkDetailRepo : GenericRepository<T_PS_VOLUME_WORK_DETAIL>, IVolumeWorkDetailRepo
    {
        public VolumeWorkDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
