using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_ROLE : BaseEntity
    {
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        [RegularExpression(@"^\S*$", ErrorMessage = "Không được phép nhập dấu cách")]
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string NOTES { get; set; }
        public virtual IList<T_AD_ROLE_DETAIL> ListRoleDetail { get; set; }
        public virtual IList<T_AD_USER_GROUP_ROLE> ListUserGroupRole { get; set; }

        public T_AD_ROLE()
        {
            ListRoleDetail = new List<T_AD_ROLE_DETAIL>();
            ListUserGroupRole = new List<T_AD_USER_GROUP_ROLE>();
        }
    }
}
