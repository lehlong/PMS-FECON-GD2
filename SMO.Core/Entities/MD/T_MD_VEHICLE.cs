using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_MD_VEHICLE : BaseEntity
    {
        [Required (ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings =false)]
        [MaxLength(length:50, ErrorMessage = "Chỉ được phép nhập tối đa {1} kí tự")]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        public virtual string TRANSUNIT_CODE { get; set; }
        public virtual string UNIT { get; set; }
        public virtual string OIC_PBATCH { get; set; }
        public virtual string OIC_PTRIP { get; set; }
        public virtual decimal CAPACITY { get; set; }
        public virtual string TRANSMODE_CODE { get; set; }
        public virtual IList<T_MD_VEHICLE_COMPARTMENT> ListCompartment { get; set; }
        public T_MD_VEHICLE()
        {
            ListCompartment = new List<T_MD_VEHICLE_COMPARTMENT>();
        }
    }
}
