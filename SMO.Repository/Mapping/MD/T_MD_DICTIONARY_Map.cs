using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_DICTIONARY_Map : BaseMapping<T_MD_DICTIONARY>
    {
        public T_MD_DICTIONARY_Map()
        {
            Table("T_MD_DICTIONARY");
            Id(x => x.PKID);
            Map(x => x.FK_DOMAIN).Not.Nullable();
            Map(x => x.CODE).Not.Nullable();
            Map(x => x.LANG).Not.Nullable();
            Map(x => x.C_VALUE).Not.Nullable();
            Map(x => x.C_ORDER).Nullable();
            Map(x => x.C_DEFAULT).Nullable().CustomType<YesNoType>();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
