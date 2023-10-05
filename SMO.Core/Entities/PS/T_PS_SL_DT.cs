using Newtonsoft.Json;

using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_SL_DT : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid TIME_PERIOD_ID { get; set; }
        public virtual string CRITERIA_CODE { get; set; }
        public virtual decimal VALUE { get; set; }

        [JsonIgnore]
        public virtual T_PS_PROJECT Project { get; set; }
        //[JsonIgnore]
        public virtual T_PS_TIME TimePeriod { get; set; }
        //[JsonIgnore]
        public virtual T_MD_CRITERIA Criteria { get; set; }
    }
}
