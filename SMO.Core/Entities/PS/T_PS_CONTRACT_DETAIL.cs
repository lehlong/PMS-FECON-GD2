using Newtonsoft.Json;

using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONTRACT_DETAIL : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid CONTRACT_ID { get; set; }
        public virtual Guid PROJECT_STRUCT_ID { get; set; }
        public virtual string UNIT_CODE { get; set; }
        public virtual string STATUS { get; set; }
        public virtual decimal VOLUME { get; set; }
        public virtual decimal UNIT_PRICE { get; set; }

        [JsonIgnore]
        public virtual T_PS_CONTRACT Contract { get; set; }
        public virtual T_PS_PROJECT_STRUCT Struct { get; set; }
        public virtual T_MD_UNIT Unit { get; set; }

    }
}
