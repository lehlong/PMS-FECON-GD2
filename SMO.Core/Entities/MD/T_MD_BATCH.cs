using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_BATCH : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [RfcStructureField("ZMANGUON")]
        public virtual string CODE { get; set; }
        [RfcStructureField("ZTENNGUON")]
        public virtual string TEXT { get; set; }
        [RfcStructureField("SHORTTEXT")]
        public virtual string TEXTS { get; set; }
    }
}
