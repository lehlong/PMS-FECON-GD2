using System;

namespace SMO.Core.Entities
{
    public partial class T_AD_FORM_OBJECT : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string FK_FORM { get; set; }
        public virtual string OBJECT_CODE { get; set; }
        public virtual string TYPE { get; set; }
    }   
}
