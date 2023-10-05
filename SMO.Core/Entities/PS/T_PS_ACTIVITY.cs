using SMO.Core.Common;

using System;
using System.Collections.Generic;

namespace SMO.Core.Entities.PS
{
    public class T_PS_ACTIVITY : BasePSEntity
    {
        public T_PS_ACTIVITY()
        {
            Tasks = new List<T_PS_TASK>();
        }
        public virtual Guid ID { get; set; }
        public virtual Guid WBS_PARENT_ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid REFRENCE_ID { get; set; }
        public virtual Guid? BOQ_REFRENCE_ID { get; set; }
        public virtual DateTime? ACTUAL_START_DATE { get; set; }
        public virtual DateTime? ACTUAL_FINISH_DATE { get; set; }
        public virtual string ASSIGNED_TO { get; set; }
        public virtual string TEXT { get; set; }
        public virtual decimal PLAN_VOLUME { get; set; }
        public virtual string PLAN_VOLUME_UNIT_ID { get; set; }
        public virtual decimal ACTUAL_VOLUME { get; set; }
        public virtual string ACTUAL_VOLUME_UNIT_ID { get; set; }
        public virtual string STATUS { get; set; }

        public virtual string TOTAL_FLOAT { get; set; }
        public virtual string FREE_FLOAT { get; set; }
        public virtual string PREDECESSOR { get; set; }
        public virtual string SUCCESSOR { get; set; }
        public virtual string RELATIONSHIP_TYPE { get; set; }
        public virtual string PURCHARING_ORG { get; set; }
        public virtual string PURCHARING_GROUP { get; set; }
        public virtual string CONTROL_KEY { get; set; }

        public virtual IList<T_PS_TASK> Tasks { get; set; }

        public virtual T_PS_PROJECT_STRUCT ReferenceBoq { get; set; }

        protected override T_PS_PROJECT_STRUCT CastToProjectStruct()
        {
            return new T_PS_PROJECT_STRUCT
            {
                ACTIVITY_ID = ID,
                WBS_ID = WBS_PARENT_ID,
                PROJECT_ID = PROJECT_ID,
                START_DATE = START_DATE,
                FINISH_DATE = FINISH_DATE,
                TEXT = TEXT,
                CREATE_BY = CREATE_BY,
                TYPE = ProjectEnum.ACTIVITY.ToString(),
                STATUS = STATUS,
            };
        }

        public static explicit operator T_PS_ACTIVITY(T_PS_PROJECT_STRUCT projectStruct)
        {
            return new T_PS_ACTIVITY
            {
                PROJECT_ID = projectStruct.PROJECT_ID,
                FINISH_DATE = projectStruct.FINISH_DATE,
                TEXT = projectStruct.TEXT,
                STATUS = projectStruct.STATUS,
                START_DATE = projectStruct.START_DATE,
                ID = projectStruct.ACTIVITY_ID.Value,
                WBS_PARENT_ID = projectStruct.PARENT_ID.Value,
                CREATE_BY = projectStruct.CREATE_BY,
            };
        }
    }
}
