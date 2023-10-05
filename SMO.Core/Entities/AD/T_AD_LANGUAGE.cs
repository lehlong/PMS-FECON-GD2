using System;

namespace SMO.Core.Entities
{
    public partial class T_AD_LANGUAGE : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string FK_CODE { get; set; }
        public virtual string OBJECT_TYPE { get; set; }
        public virtual string FORM_CODE { get; set; }
        public virtual string LANG { get; set; }
        public virtual string VALUE { get; set; }
    }   
}
