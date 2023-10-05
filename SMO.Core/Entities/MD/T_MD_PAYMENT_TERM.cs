using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_PAYMENT_TERM : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [RfcStructureField("ZTERM")]
        public virtual string CODE { get; set; }
        [RfcStructureField("TEXT1")]
        public virtual string TEXT { get; set; }
        [RfcStructureField("ZTAG1")]
        public virtual int PAYMENT_TERM_D { get; set; }
        [RfcStructureField("ZSTG1")]
        public virtual int PAYMENT_TERM_M { get; set; }
    }
}
