using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace SMO.Core.Entities.PS
{
    public class T_PS_PROJECT_WORKFLOW_COMMENT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid STEP_ID { get; set; }
        public virtual String COMMENT { get; set; }

    }
}
