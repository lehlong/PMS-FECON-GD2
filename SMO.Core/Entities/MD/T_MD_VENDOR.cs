using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    [Serializable]
    public class T_MD_VENDOR : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [MaxLength(length: 50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string NAME { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string MST { get; set; }
        public virtual string SHORT_NAME { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string PHONE { get; set; }
        public virtual string ADDRESS { get; set; }
    }
}
