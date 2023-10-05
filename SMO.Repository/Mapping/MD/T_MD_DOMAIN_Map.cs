using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_DOMAIN_Map : BaseMapping<T_MD_DOMAIN>
    {
        public T_MD_DOMAIN_Map()
        {
            Table("T_MD_DOMAIN");
            Id(x => x.CODE);
            Map(x => x.NAME).Nullable();
            Map(x => x.DATA_TYPE).Nullable();
            Map(x => x.NOTE).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
            HasMany(x => x.ListDictionary).KeyColumn("FK_DOMAIN").Inverse().Cascade.All();
        }
    }
}
