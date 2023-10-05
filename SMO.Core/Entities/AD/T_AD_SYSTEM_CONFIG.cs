using System;

namespace SMO.Core.Entities
{
    public partial class T_AD_SYSTEM_CONFIG : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string AD_CONNECTION { get; set; }
        public virtual string SAP_HOST { get; set; }
        public virtual string SAP_CLIENT { get; set; }
        public virtual string SAP_NUMBER { get; set; }
        public virtual string SAP_USER_NAME { get; set; }
        public virtual string SAP_PASSWORD { get; set; }
        public virtual int SAP_TIME_DIFF { get; set; }
        public virtual string TGBX_URL { get; set; }
        public virtual string TGBX_USER_NAME { get; set; }
        public virtual string TGBX_PASSWORD { get; set; }
        public virtual string TGBX_API_URL { get; set; }
        public virtual string TGBX_API_USER_NAME { get; set; }
        public virtual string TGBX_API_PASSWORD { get; set; }
        public virtual int TGBX_TIME_DIFF { get; set; }
        public virtual string SMO_API_USER_NAME { get; set; }
        public virtual string SMO_API_PASSWORD { get; set; }
        public virtual string SMO_API_LINK_SMO { get; set; }
        public virtual string SMS_WEBSERVICE { get; set; }
        public virtual string SMS_APP { get; set; }
        public virtual string SMS_PASSWORD { get; set; }
        public virtual string SMS_BRAND_NAME { get; set; }

        public virtual string MAIL_HOST { get; set; }
        public virtual string MAIL_SMTPHOST { get; set; }
        public virtual int MAIL_PORT { get; set; }
        public virtual string MAIL_USER { get; set; }
        public virtual string MAIL_PASSWORD { get; set; }
        public virtual bool MAIL_IS_SSL { get; set; }

        public virtual string EGAS_USER_NAME { get; set; }
        public virtual string EGAS_PASSWORD { get; set; }
        public virtual string EGAS_WEBSERVICE { get; set; }

        public virtual int JOB_STATUS_SAP { get; set; }
        public virtual int JOB_STATUS_TGBX { get; set; }
        public virtual int JOB_SEND_EMAIL { get; set; }
        public virtual int JOB_SEND_SMS { get; set; }

        public virtual int XTTD_TIME_DIFF { get; set; }
        public virtual DateTime? LAST_SYN_VEHICLE { get; set; }

        public virtual string CURRENT_CONNECTION { get; set; }
        public virtual string DIRECTORY_CACHE { get; set; }
        public virtual T_AD_CONNECTION Connection { get; set; }
    }
}
