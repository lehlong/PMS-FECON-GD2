using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CM_SMS_OTP : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string MODUL_TYPE { get; set; }
        public virtual string OTP_CODE { get; set; }
        public virtual DateTime EFFECT_TIME { get; set; }
    }
}
