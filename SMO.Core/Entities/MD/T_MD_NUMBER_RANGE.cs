using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    public class T_MD_NUMBER_RANGE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual int CURRENT_NUMBER { get; set; }
        public virtual string CHARACTER { get; set; }
        public virtual string IS_DELETE { get; set; }
    }
}
