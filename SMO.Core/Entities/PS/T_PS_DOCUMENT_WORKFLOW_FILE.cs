using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_DOCUMENT_WORKFLOW_FILE : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string WORKFLOW_CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual Guid DOCUMENT_ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }

    }
}