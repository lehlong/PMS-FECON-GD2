using SMO.Core.Entities.PS;

using System;
using System.Runtime.Serialization;

namespace SMO.AppCode.GanttChart
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid? ReferenceBoqId { get; set; }
        public string Text { get; set; }
        [DataMember(Name = "start_date")]
        public DateTime Start_date { get; set; }
        public DateTime End_date { get; set; }
        public int Duration { get; set; }
        public double Order { get; set; }
        public decimal? Progress { get; set; }
        public Guid? Parent { get; set; }
        public string Type { get; set; }
        public string Target { get; set; }
        public decimal? Plan_volume { get; set; }
        public decimal? Actual_volume { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }
        public string UnitCode { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string User { get; set; }
        public string Code { get; set; }
        public string ContractCode { get; set; }
        public string VendorName { get; set; }

        public static implicit operator T_PS_PROJECT_STRUCT(TaskDto taskDto)
        {
            var createNewTask = taskDto.Id == Guid.Empty;
            Guid? wbsId = null;
            Guid? boqId = null;
            if (createNewTask)
            {
                if (taskDto.Type == ProjectEnum.WBS.ToString())
                {
                    wbsId = Guid.NewGuid();
                }
                else if (taskDto.Type == ProjectEnum.ACTIVITY.ToString())
                {
                    wbsId = taskDto.Parent;
                }
                else if (taskDto.Type == ProjectEnum.BOQ.ToString())
                {
                    boqId = Guid.NewGuid();
                }
            } else
            {
                if (taskDto.Type == ProjectEnum.WBS.ToString() || taskDto.Type == ProjectEnum.ACTIVITY.ToString())
                {
                    wbsId = taskDto.Parent;
                }
                else if (taskDto.Type == ProjectEnum.BOQ.ToString())
                {
                    boqId = taskDto.Parent;
                }
            }
            return new T_PS_PROJECT_STRUCT
            {
                ID=taskDto.Id,  
                TYPE = taskDto.Type,
                START_DATE = taskDto.Start_date,
                FINISH_DATE = taskDto.End_date,
                PARENT_ID = taskDto.Parent,
                TEXT = taskDto.Text,
                WBS_ID = wbsId,
                BOQ_ID = boqId,
                PROJECT_ID = taskDto.ProjectId,
                UNIT_CODE = taskDto.UnitCode,
                QUANTITY = taskDto.Quantity,
                PRICE = taskDto.Price,
                C_ORDER = taskDto.Order,
                GEN_CODE = taskDto.Code,
                STATUS = taskDto.Status,
            };
        }

        public static explicit operator TaskDto(T_PS_PROJECT_STRUCT projectStruct)
        {
            var referenceBoqId = projectStruct.TYPE == ProjectEnum.WBS.ToString() ?
                projectStruct.Wbs?.BOQ_REFRENCE_ID : projectStruct.Activity?.BOQ_REFRENCE_ID;
            return new TaskDto
            {
                Id = projectStruct.ID,
                Text = projectStruct.TEXT,
                Start_date = projectStruct.START_DATE,
                End_date = projectStruct.FINISH_DATE,
                ProjectId = projectStruct.PROJECT_ID,
                Parent = projectStruct.PARENT_ID,
                Order = projectStruct.C_ORDER,
                Type = projectStruct.TYPE,
                UnitCode = projectStruct.UNIT_CODE,
                Quantity = projectStruct.QUANTITY,
                Price = projectStruct.PRICE,
                Code = projectStruct.GEN_CODE,
                ReferenceBoqId = referenceBoqId,
                Status = projectStruct.STATUS,
            };
        }
    }

}
