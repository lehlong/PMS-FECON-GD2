using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_ORGANIZE_Map : BaseMapping<T_AD_ORGANIZE>
    {
        public T_AD_ORGANIZE_Map()
        {
            Table("T_AD_ORGANIZE");
            Id(x => x.PKID);
            Map(x => x.PARENT);
            Map(x => x.NAME).Not.Nullable();
            Map(x => x.TYPE);
            Map(x => x.COMPANY_CODE);
            Map(x => x.COST_CENTER_CODE);
            Map(x => x.C_ORDER);
        }
    }
}
