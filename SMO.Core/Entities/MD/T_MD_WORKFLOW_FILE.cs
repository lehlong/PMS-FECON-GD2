using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    public class T_MD_WORKFLOW_FILE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string WORKFLOW_CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual int C_ORDER { get; set; }

    }
}
