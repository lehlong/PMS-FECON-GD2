using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_MATERIAL : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        [RfcStructureField("MATNR")]
        public virtual string CODE { get; set; }
        [RfcStructureField("MAKTX")]
        public virtual string TEXT { get; set; }
        [RfcStructureField("MTART")]
        public virtual string TYPE { get; set; }
        [RfcStructureField("MEINS")]
        public virtual string UNIT { get; set; }
    }
}
