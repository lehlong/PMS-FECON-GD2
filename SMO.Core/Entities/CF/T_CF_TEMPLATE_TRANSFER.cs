using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SMO.Core.Entities
{
    public partial class T_CF_TEMPLATE_TRANSFER : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string SMS_SOURCE_TEMPLATE { get; set; }
        public virtual string SUBJECT_SOURCE_TEMPLATE { get; set; }
        public virtual string EMAIL_SOURCE_TEMPLATE { get; set; }
        public virtual string SMS_DES_TEMPLATE { get; set; }
        public virtual string SUBJECT_DES_TEMPLATE { get; set; }
        public virtual string EMAIL_DES_TEMPLATE { get; set; }
    }
}
