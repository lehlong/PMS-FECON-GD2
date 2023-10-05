using SMO.Core.Entities;
using SMO.Core.Entities.PS;

using System;

namespace SMO.Core.Common
{
    public class BaseProjectPlanEntity : BaseEntity
    {
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid PROJECT_STRUCT_ID { get; set; }
        public virtual decimal VALUE { get; set; }

        public virtual bool IS_CUSTOMER { get; set; }
        public virtual T_PS_PROJECT Project { get; set; }
        public virtual T_PS_PROJECT_STRUCT ProjectStruct { get; set; }
    }
}
