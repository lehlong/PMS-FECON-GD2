using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_STORE_Map : BaseMapping<T_MD_STORE>
    {
        public T_MD_STORE_Map()
        {
            Table("T_MD_STORE");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
            HasMany(x => x.ListStoreMaterial).KeyColumn("STORE_CODE").LazyLoad().Inverse().Cascade.All();
        }
    }
}
