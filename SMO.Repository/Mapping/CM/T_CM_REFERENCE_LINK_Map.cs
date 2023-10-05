using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_REFERENCE_LINK_Map : BaseMapping<T_CM_REFERENCE_LINK>
    {
        public T_CM_REFERENCE_LINK_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned(); ;
            Map(x => x.REFERENCE_ID);
            Map(x => x.LINK);
        }
    }
}
