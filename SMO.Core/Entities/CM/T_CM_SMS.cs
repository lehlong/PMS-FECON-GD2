using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CM_SMS : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string PO_CODE { get; set; }
        public virtual string MODUL_TYPE { get; set; }
        public virtual string PHONE_NUMBER { get; set; }
        public virtual string USER_RECEIVED { get; set; }
        public virtual string CONTENTS { get; set; }
        public virtual bool IS_SEND { get; set; }
        public virtual int NUMBER_RETRY { get; set; }
    }
}
