using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_AD_FORM : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string NOTES { get; set; }
        public virtual IList<T_AD_FORM_OBJECT> ListObject { get; set; }
    }   
}
