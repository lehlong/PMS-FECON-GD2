using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_SHPOINT_Map : BaseMapping<T_MD_SHPOINT>
    {
        public T_MD_SHPOINT_Map()
        {
            Table("T_MD_SHPOINT");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
