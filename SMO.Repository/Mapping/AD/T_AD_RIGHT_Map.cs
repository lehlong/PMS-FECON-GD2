using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_RIGHT_Map : BaseMapping<T_AD_RIGHT>
    {
        public T_AD_RIGHT_Map()
        {
            Table("T_AD_RIGHT");
            Id(x => x.CODE);
            Map(x => x.NAME).Not.Nullable();
            Map(x => x.PARENT).Nullable();
            Map(x => x.C_ORDER).Nullable();
            HasMany(x => x.ListRoleDetail).KeyColumn("FK_RIGHT").Inverse().Cascade.Delete();
            HasMany(x => x.ListUserRight).KeyColumn("FK_RIGHT").Inverse().Cascade.Delete();
        }
    }
}
