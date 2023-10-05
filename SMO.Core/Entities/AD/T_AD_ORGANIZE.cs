using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_ORGANIZE : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string PARENT { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập")]
        public virtual string NAME { get; set; }
        public virtual string TYPE { get; set; }
        public virtual string COMPANY_CODE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual string COST_CENTER_CODE { get; set; }
    }   
}
