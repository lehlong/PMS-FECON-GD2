using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_AD_CONNECTION : BaseEntity
    {
        public virtual string PKID { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string NAME { get; set; }
        public virtual string ADDRESS { get; set; }
        public virtual string USERNAME { get; set; }
        public virtual string PASSWORD { get; set; }
        [Required(ErrorMessage = "Trường này bắt buộc nhập", AllowEmptyStrings = false)]
        public virtual string DIRECTORY { get; set; }
        public virtual string NOTES { get; set; }
    }   
}
