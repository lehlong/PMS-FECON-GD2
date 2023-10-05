using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_STORE : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string TEXT { get; set; }
        public virtual IList<T_CF_STORE_MATERIAL> ListStoreMaterial { get; set; }

        public T_MD_STORE()
        {
            ListStoreMaterial = new List<T_CF_STORE_MATERIAL>();
        }
    }
}
