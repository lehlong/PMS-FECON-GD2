using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_MENU_Map : BaseMapping<T_AD_MENU>
    {
        public T_AD_MENU_Map()
        {
            Table("T_AD_MENU");
            Id(x => x.CODE);
            Map(x => x.DESCRIPTION).Nullable();
            Map(x => x.PARENT).Nullable();
            Map(x => x.C_ORDER).Nullable();
            Map(x => x.FK_RIGHT).Nullable();
            Map(x => x.LINK).Nullable();
            Map(x => x.ICON).Nullable();
        }
    }
}
