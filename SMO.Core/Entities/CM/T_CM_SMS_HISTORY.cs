using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CM_SMS_HISTORY : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string TYPE { get; set; }
        public virtual string HEADER_ID { get; set; }
        public virtual string PHONE { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual string MESSAGE { get; set; }
        public virtual bool IS_SEND { get; set; }

        // Trường phục vụ việc search
        public virtual DateTime? FROM_DATE { get; set; }
        public virtual DateTime? TO_DATE { get; set; }
    }
}
