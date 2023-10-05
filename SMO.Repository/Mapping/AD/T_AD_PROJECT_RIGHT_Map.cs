using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_PROJECT_RIGHT_Map : BaseMapping<T_AD_PROJECT_RIGHT>
    {
        public T_AD_PROJECT_RIGHT_Map()
        {
            Table("T_AD_PROJECT_RIGHT");
            Id(x => x.CODE);
            Map(x => x.NAME).Not.Nullable();
            Map(x => x.PARENT).Nullable();
            Map(x => x.C_ORDER).Nullable();
        }
    }
}
