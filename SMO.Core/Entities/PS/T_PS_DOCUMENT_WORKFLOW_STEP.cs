using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.PS
{
    public class T_PS_DOCUMENT_WORKFLOW_STEP : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string WORKFLOW_CODE { get; set; }
        public virtual Guid WORKFLOW_ID { get; set; }
        public virtual string NAME { get; set; }
        public virtual string PROJECT_ROLE_CODE { get; set; }

        public virtual string USER_ACTION { get; set; }

        public virtual int NUMBER_DAYS { get; set; }
        public virtual string ACTION { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual bool IS_DONE { get; set; }
        public virtual Guid DOCUMENT_ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
    }
}