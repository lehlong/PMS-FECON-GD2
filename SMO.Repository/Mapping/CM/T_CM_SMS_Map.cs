using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_SMS_Map : BaseMapping<T_CM_SMS>
    {
        public T_CM_SMS_Map()
        {
            Table("T_CM_SMS");
            Id(x => x.PKID);
            Map(x => x.PO_CODE);
            Map(x => x.MODUL_TYPE);
            Map(x => x.PHONE_NUMBER);
            Map(x => x.USER_RECEIVED);
            Map(x => x.CONTENTS);
            Map(x => x.NUMBER_RETRY);
            Map(x => x.IS_SEND).CustomType<YesNoType>();
        }
    }
}
