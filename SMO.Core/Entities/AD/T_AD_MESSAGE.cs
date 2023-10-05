using System;

namespace SMO.Core.Entities
{
    public partial class T_AD_MESSAGE : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string CODE { get; set; }
        public virtual string LANGUAGE { get; set; }
        public virtual string MESSAGE { get; set; }
#pragma warning disable CS0114 // 'T_AD_MESSAGE.ACTIVE' hides inherited member 'BaseEntity.ACTIVE'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public virtual bool ACTIVE { get; set; }
#pragma warning restore CS0114 // 'T_AD_MESSAGE.ACTIVE' hides inherited member 'BaseEntity.ACTIVE'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
    }   
}
