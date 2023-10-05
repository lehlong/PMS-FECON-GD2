using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_DIVISION : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [RfcStructureField("SPART")]
        public virtual string CODE { get; set; }
        [RfcStructureField("VTEXT")]
        public virtual string TEXT { get; set; }
        [RfcStructureField("VKORG")]
        public virtual string COMPANY_CODE { get; set; }
    }
}
