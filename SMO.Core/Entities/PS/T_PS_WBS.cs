using SMO.Core.Common;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_WBS : BasePSEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid? WBS_PARENT_ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid REFRENCE_ID { get; set; }
        public virtual Guid? BOQ_REFRENCE_ID { get; set; }
        public virtual DateTime? ACTUAL_START_DATE { get; set; }
        public virtual DateTime? ACTUAL_FINISH_DATE { get; set; }
        public virtual string TEXT { get; set; }
        public virtual decimal PLAN_VOLUME { get; set; }
        public virtual string PLAN_VOLUME_UNIT_ID { get; set; }
        public virtual decimal ACTUAL_VOLUME { get; set; }
        public virtual string ACTUAL_VOLUME_UNIT_ID { get; set; }
        public virtual string STATUS { get; set; }
        public virtual string PLAN_METHOD { get; set; }
        public virtual string PRIORITY { get; set; }
        public virtual string PLANT { get; set; }

        public virtual T_PS_PROJECT_STRUCT ReferenceBoq { get; set; }
        protected override T_PS_PROJECT_STRUCT CastToProjectStruct()
        {
            return new T_PS_PROJECT_STRUCT
            {
                WBS_ID = ID,
                PROJECT_ID = PROJECT_ID,
                START_DATE = START_DATE,
                FINISH_DATE = FINISH_DATE,
                TEXT = TEXT,
                CREATE_BY = CREATE_BY,
                TYPE = ProjectEnum.WBS.ToString(),
                STATUS = STATUS,
            };
        }

        public static explicit operator T_PS_WBS(T_PS_PROJECT_STRUCT projectStruct)
        {
            return new T_PS_WBS {
                PROJECT_ID = projectStruct.PROJECT_ID,
                FINISH_DATE = projectStruct.FINISH_DATE,
                TEXT = projectStruct.TEXT,
                STATUS = projectStruct.STATUS,
                START_DATE = projectStruct.START_DATE,
                ID = projectStruct.WBS_ID.Value,
                WBS_PARENT_ID = projectStruct.PARENT_ID == projectStruct.PROJECT_ID ? null : projectStruct.PARENT_ID,
                CREATE_BY = projectStruct.CREATE_BY,
            };
        }
    }
}
