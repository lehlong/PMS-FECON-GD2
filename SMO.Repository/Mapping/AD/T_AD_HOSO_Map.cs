using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_HOSO_Map : BaseMapping<T_AD_HOSO>
    {
        public T_AD_HOSO_Map()
        {
            Table("T_AD_HOSO");
            Id(x => x.PKID);
            Map(x => x.USERNAME);
            Map(x => x.INFO1);
            Map(x => x.INFO2);
            Map(x => x.INFO3);
            Map(x => x.INFO4);
            Map(x => x.INFO5);
            Map(x => x.INFO6);
            Map(x => x.INFO7);
            Map(x => x.INFO8);
            Map(x => x.INFO9);
            Map(x => x.INFO10);
        }
    }
}
