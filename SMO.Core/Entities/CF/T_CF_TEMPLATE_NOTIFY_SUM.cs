using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SMO.Core.Entities
{
    public partial class T_CF_TEMPLATE_NOTIFY_SUM : BaseEntity
    {
        public virtual string PO_STATUS { get; set; }
        public virtual string SMS_TEMPLATE { get; set; }
        public virtual string SUBJECT_TEMPLATE { get; set; }
        public virtual string EMAIL_TEMPLATE { get; set; }
    }
}
