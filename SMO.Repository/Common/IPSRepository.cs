using SMO.Core.Common;

namespace SMO.Repository.Common
{
    public interface IPSRepository<T> : IGenericRepository<T> where T : BasePSEntity
    {

    }
}
