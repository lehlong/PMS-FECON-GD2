using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_ROLE_Map : BaseMapping<T_AD_ROLE>
    {
        public T_AD_ROLE_Map()
        {
            Table("T_AD_ROLE");
            Id(x => x.CODE);
            Map(x => x.NAME).Not.Nullable();
            Map(x => x.NOTES).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
            HasMany(x => x.ListRoleDetail).KeyColumn("FK_ROLE").Inverse().Cascade.Delete();
            HasMany(x => x.ListUserGroupRole).KeyColumn("ROLE_CODE").Inverse().Cascade.All();
        }
    }
}
