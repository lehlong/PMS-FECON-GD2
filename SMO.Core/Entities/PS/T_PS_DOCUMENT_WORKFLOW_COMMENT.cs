using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_DOCUMENT_WORKFLOW_COMMENT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid STEP_ID { get; set; }
        public virtual string COMMENT { get; set; }
    }
}