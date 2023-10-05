using SMO.Core.Common;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_BOQ : BasePSEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string TEXT { get; set; }
        public virtual decimal PLAN_VOLUME { get; set; }
        public virtual string PLAN_VOLUME_UNIT_ID { get; set; }
        public virtual decimal ACTUAL_VOLUME { get; set; }
        public virtual string ACTUAL_VOLUME_UNIT_ID { get; set; }
        public virtual string STATUS { get; set; }
        public virtual DateTime? ACTUAL_START_DATE { get; set; }
        public virtual DateTime? ACTUAL_FINISH_DATE { get; set; }
        protected override T_PS_PROJECT_STRUCT CastToProjectStruct()
        {
            return new T_PS_PROJECT_STRUCT
            {
                BOQ_ID = ID,
                PROJECT_ID = PROJECT_ID,
                START_DATE = START_DATE,
                FINISH_DATE = FINISH_DATE,
                TEXT = TEXT,
                CREATE_BY = CREATE_BY,
                TYPE = ProjectEnum.BOQ.ToString(),
                STATUS = STATUS,
            };
        }

        public static explicit operator T_PS_BOQ(T_PS_PROJECT_STRUCT projectStruct)
        {
            return new T_PS_BOQ
            {
                PROJECT_ID = projectStruct.PROJECT_ID,
                FINISH_DATE = projectStruct.FINISH_DATE,
                TEXT = projectStruct.TEXT,
                STATUS = projectStruct.STATUS,
                START_DATE = projectStruct.START_DATE,
                ID = projectStruct.BOQ_ID.Value,
                CREATE_BY = projectStruct.CREATE_BY,
            };
        }
    }
}
