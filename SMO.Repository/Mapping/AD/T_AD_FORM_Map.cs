using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_FORM_Map : BaseMapping<T_AD_FORM>
    {
        public T_AD_FORM_Map()
        {
            Table("T_AD_FORM");
            Id(x => x.CODE);
            Map(x => x.NAME).Not.Nullable();
            Map(x => x.NOTES).Nullable();
            HasMany(x => x.ListObject).KeyColumn("FK_FORM").Inverse().Cascade.All();
        }
    }
}
