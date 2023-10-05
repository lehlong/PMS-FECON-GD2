using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_TIME : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual DateTime START_DATE { get; set; }
        public virtual DateTime FINISH_DATE { get; set; }
        public virtual string TIME_TYPE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual int MONTH { get; set; }
        public virtual int YEAR { get; set; }

        public virtual T_PS_PROJECT Project { get; set; }
        public virtual T_MD_TIME_TYPE TimeType { get; set; }
    }
}
