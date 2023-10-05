using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_USER_HISTORY : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual DateTime LOGON_TIME { get; set; }
        public virtual string BROWSER { get; set; }
        public virtual string VERSION { get; set; }
        public virtual string OS { get; set; }
        public virtual string MOBILE_MODEL { get; set; }
        public virtual string MANUFACTURER { get; set; }
        public virtual bool IS_MOBILE { get; set; }
        public virtual string IP_ADDRESS { get; set; }
        public virtual bool STATUS { get; set; }
    }
}
