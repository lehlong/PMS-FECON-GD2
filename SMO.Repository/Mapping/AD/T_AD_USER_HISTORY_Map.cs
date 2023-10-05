using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_USER_HISTORY_Map : BaseMapping<T_AD_USER_HISTORY>
    {
        public T_AD_USER_HISTORY_Map()
        {
            Table("T_AD_USER_HISTORY");
            Id(x => x.PKID);
            Map(x => x.USER_NAME);
            Map(x => x.LOGON_TIME);
            Map(x => x.BROWSER);
            Map(x => x.VERSION);
            Map(x => x.OS);
            Map(x => x.MOBILE_MODEL);
            Map(x => x.MANUFACTURER);
            Map(x => x.IP_ADDRESS);
            Map(x => x.IS_MOBILE).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.STATUS).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
