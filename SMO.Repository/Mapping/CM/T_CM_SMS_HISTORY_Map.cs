using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_SMS_HISTORY_Map : BaseMapping<T_CM_SMS_HISTORY>
    {
        public T_CM_SMS_HISTORY_Map()
        {
            Table("T_CM_SMS_HISTORY");
            Id(x => x.PKID);
            Map(x => x.TYPE);
            Map(x => x.HEADER_ID);
            Map(x => x.PHONE);
            Map(x => x.CONTENTS);
            Map(x => x.MESSAGE);
            Map(x => x.IS_SEND).CustomType<YesNoType>();
        }
    }
}
