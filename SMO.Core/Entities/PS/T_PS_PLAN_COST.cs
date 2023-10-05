using SMO.Core.Common;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_PLAN_COST : BaseProjectPlanEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid? CONTRACT_DETAIL_ID { get; set; }
        public virtual Guid TIME_PERIOD_ID { get; set; }
        public virtual decimal TOTAL { get; set; }

        public virtual T_PS_CONTRACT_DETAIL ContractDetail { get; set; }
        public virtual T_PS_TIME TimePeriod { get; set; }
    }
}
