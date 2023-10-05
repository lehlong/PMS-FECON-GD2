using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_MATERIAL_Map : BaseMapping<T_MD_MATERIAL>
    {
        public T_MD_MATERIAL_Map()
        {
            Table("T_MD_MATERIAL");
            Id(x => x.CODE);
            Map(x => x.TEXT);
            Map(x => x.TYPE);
            Map(x => x.UNIT);
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
