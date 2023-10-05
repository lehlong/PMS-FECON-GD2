using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CM_NOTIFY : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual string RAW_CONTENTS { get; set; }
        public virtual bool IS_REAED { get; set; }
        public virtual bool IS_COUNTED { get; set; }
    }
}
