using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_HISTORY_SMO_API_Map : BaseMapping<T_CM_HISTORY_SMO_API>
    {
        public T_CM_HISTORY_SMO_API_Map()
        {
            Table("T_CM_HISTORY_SMO_API");
            Id(x => x.ID).GeneratedBy.Identity();
            Map(x => x.C_FUNCTION);
            Map(x => x.PARAMETER);
            Map(x => x.RESULT);
        }
    }
}
