using NHibernate;

using SMO.Core.Common;

namespace SMO.Repository.Common
{
    public abstract class PSRepository<T> : GenericRepository<T> where T : BasePSEntity
    {
        protected PSRepository(ISession _session) : base(_session)
        {
        }
    }
}
