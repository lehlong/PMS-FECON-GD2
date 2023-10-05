using SMO.Core.Entities;
using SMO.Core.Entities.PS;

using System;

namespace SMO.Core.Common
{
    public abstract class BasePSEntity : BaseEntity
    {
        public virtual DateTime START_DATE { get; set; } = DateTime.Now;
        public virtual DateTime FINISH_DATE { get; set; } = DateTime.Now;
        public virtual string PEOPLE_RESPONSIBILITY { get; set; }
        public virtual string CODE { get; set; }
        public virtual Guid? REFERENCE_FILE_ID { get; set; }

        public static explicit operator T_PS_PROJECT_STRUCT(BasePSEntity ps)
        {
            return ps.CastToProjectStruct();
        }
        

        protected abstract T_PS_PROJECT_STRUCT CastToProjectStruct();
    }
}
