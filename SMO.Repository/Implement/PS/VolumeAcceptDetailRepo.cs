using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class VolumeAcceptDetailRepo : GenericRepository<T_PS_VOLUME_ACCEPT_DETAIL>, IVolumeAcceptDetailRepo
    {
        public VolumeAcceptDetailRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
