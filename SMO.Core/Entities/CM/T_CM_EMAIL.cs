using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CM_EMAIL : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string STATUS { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string SUBJECT { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual bool IS_SEND { get; set; }
        public virtual int NUMBER_RETRY { get; set; }
    }
}
