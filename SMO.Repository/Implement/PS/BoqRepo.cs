using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

namespace SMO.Repository.Implement.PS
{
    public class BoqRepo : PSRepository<T_PS_BOQ>, IBoqRepo
    {
        public BoqRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
