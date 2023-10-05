using SMO.Core.Interface;
using System;
using Newtonsoft.Json;

namespace SMO.Core.Entities
{
    [Serializable]
    public abstract class BaseEntity : IEntity
    {
        public virtual string CREATE_BY { get; set; }
        [JsonIgnore]
        public virtual T_AD_USER USER_CREATE { get; set; }
        public virtual DateTime? CREATE_DATE { get; set; }
        public virtual string UPDATE_BY { get; set; }
        [JsonIgnore]
        public virtual T_AD_USER USER_UPDATE { get; set; }
        public virtual DateTime? UPDATE_DATE { get; set; }
        public virtual bool ACTIVE { get; set; }
    }
}
