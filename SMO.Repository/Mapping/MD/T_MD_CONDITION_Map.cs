using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CONDITION_Map : BaseMapping<T_MD_CONDITION>
    {
        public T_MD_CONDITION_Map()
        {
            Table("T_MD_CONDITION");
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.TYPE);
            Map(x => x.MHGL_ALLOW_EDIT).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.MHGL_ALLOW_SHOW).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.XBND_ALLOW_EDIT).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.XBND_ALLOW_SHOW).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.XBTX_ALLOW_EDIT).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.XBTX_ALLOW_SHOW).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.TEST_ALLOW_EDIT).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.TEST_ALLOW_SHOW).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
