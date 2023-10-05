using SMO.Core.Common;

namespace SMO.Service.Common
{
    public interface IPSService<T> : IGenericService<T> where T : BasePSEntity

    {

    }
}
