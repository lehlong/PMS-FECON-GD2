using System;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities.MD
{
    public class T_MD_WORKFLOW_STEP : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string WORKFLOW_CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string PROJECT_ROLE_CODE { get; set; }

        public virtual string USER_ACTION { get; set; }

        public virtual int NUMBER_DAYS { get; set; }
        public virtual string ACTION { get; set; }
    }
}
