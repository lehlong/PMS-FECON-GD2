using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    public class T_MD_WORKFLOW : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [MaxLength(length: 50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string NAME { get; set; }
        public virtual string REQUEST_TYPE_CODE { get; set; }
        public virtual string PROJECT_LEVEL_CODE { get; set; }
        public virtual decimal CONTRACT_VALUE_MIN { get; set; }
        public virtual decimal CONTRACT_VALUE_MAX  { get; set; }
        public virtual string PURCHASE_TYPE_CODE { get; set; }
        public virtual bool AUTHORITY { get; set; }
    }
}
