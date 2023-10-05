using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_SMS_OTP_Map : BaseMapping<T_CM_SMS_OTP>
    {
        public T_CM_SMS_OTP_Map()
        {
            Table("T_CM_SMS_OTP");
            Id(x => x.PKID);
            Map(x => x.USER_NAME).Not.Nullable();
            Map(x => x.MODUL_TYPE).Not.Nullable();
            Map(x => x.OTP_CODE).Not.Nullable();
            Map(x => x.EFFECT_TIME).Not.Nullable();
        }
    }
}
