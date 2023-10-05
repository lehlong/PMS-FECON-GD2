using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_SYSTEM_CONFIG_Map : BaseMapping<T_AD_SYSTEM_CONFIG>
    {
        public T_AD_SYSTEM_CONFIG_Map()
        {
            Table("T_AD_SYSTEM_CONFIG");
            Id(x => x.PKID);
            Map(x => x.AD_CONNECTION);
            Map(x => x.SAP_HOST);
            Map(x => x.SAP_CLIENT);
            Map(x => x.SAP_NUMBER);
            Map(x => x.SAP_USER_NAME);
            Map(x => x.SAP_PASSWORD);
            Map(x => x.SAP_TIME_DIFF);
            Map(x => x.TGBX_URL);
            Map(x => x.TGBX_USER_NAME);
            Map(x => x.TGBX_PASSWORD);
            Map(x => x.TGBX_API_URL);
            Map(x => x.TGBX_API_USER_NAME);
            Map(x => x.TGBX_API_PASSWORD);
            Map(x => x.TGBX_TIME_DIFF);
            Map(x => x.SMO_API_USER_NAME);
            Map(x => x.SMO_API_PASSWORD);
            Map(x => x.SMO_API_LINK_SMO);
            Map(x => x.SMS_WEBSERVICE);
            Map(x => x.SMS_BRAND_NAME);
            Map(x => x.SMS_APP);
            Map(x => x.SMS_PASSWORD);
            Map(x => x.MAIL_HOST);
            Map(x => x.MAIL_SMTPHOST);
            Map(x => x.MAIL_PASSWORD);
            Map(x => x.MAIL_PORT);
            Map(x => x.MAIL_USER);
            Map(x => x.MAIL_IS_SSL).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.EGAS_PASSWORD);
            Map(x => x.EGAS_USER_NAME);
            Map(x => x.EGAS_WEBSERVICE);

            Map(x => x.JOB_STATUS_SAP);
            Map(x => x.JOB_STATUS_TGBX);
            Map(x => x.JOB_SEND_EMAIL);
            Map(x => x.JOB_SEND_SMS);
            Map(x => x.XTTD_TIME_DIFF);
            Map(x => x.LAST_SYN_VEHICLE);
            Map(x => x.CURRENT_CONNECTION);
            Map(x => x.DIRECTORY_CACHE);
            References(x => x.Connection).Column("CURRENT_CONNECTION").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
