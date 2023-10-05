using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_AD_USER_GROUP_Map : BaseMapping<T_AD_USER_GROUP>
    {
        public T_AD_USER_GROUP_Map()
        {
            Table("T_AD_USER_GROUP");
            Id(x => x.CODE);
            Map(x => x.NAME).Nullable();
            Map(x => x.NOTES).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
            HasMany(x => x.ListUserUserGroup).KeyColumn("USER_GROUP_CODE").LazyLoad().Inverse().Cascade.All();
            HasMany(x => x.ListUserGroupRole).KeyColumn("USER_GROUP_CODE").LazyLoad().Inverse().Cascade.All();
        }
    }
}
