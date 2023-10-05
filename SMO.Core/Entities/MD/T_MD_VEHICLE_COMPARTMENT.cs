using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_VEHICLE_COMPARTMENT : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        public virtual string VEHICLE_CODE { get; set; }
        public virtual string SEQ_NUMBER { get; set; }
        public virtual decimal CAPACITY { get; set; }
    }
}
