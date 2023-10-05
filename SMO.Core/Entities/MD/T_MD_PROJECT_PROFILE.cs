using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    public class T_MD_PROJECT_PROFILE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string COMPANY_CODE { get; set; }
        public virtual string PROJECT_PROFILE { get; set; }
        public virtual string PROJECT_TYPE { get; set; }
        public virtual string FIRST_CHARACTER { get; set; }
        public virtual string IS_DELETE { get; set; }
    }
}
