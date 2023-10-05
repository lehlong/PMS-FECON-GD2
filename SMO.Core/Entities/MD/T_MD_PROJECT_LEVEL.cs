using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    [Serializable]
    public class T_MD_PROJECT_LEVEL : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [MaxLength(length: 50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string NAME { get; set; }
        public virtual string VALUE_FROM { get; set; }
        public virtual string VALUE_TO { get; set; }
        public virtual string THOI_GIAN { get; set; }
        public virtual string NOTES { get; set; }
    }
}
