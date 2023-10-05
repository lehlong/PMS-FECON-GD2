using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_SLOCCH : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [RfcStructureField("LGORT")]
        public virtual string CODE { get; set; }
        [RfcStructureField("LGOBE")]
        public virtual string TEXT { get; set; }
    }
}
