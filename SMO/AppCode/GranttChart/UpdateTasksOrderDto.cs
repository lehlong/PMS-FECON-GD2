using System;
using System.Collections.Generic;

namespace SMO.AppCode.GanttChart
{
    public class UpdateTasksOrderDto
    {
        public UpdateTasksOrderDto()
        {
            TaskIds = new List<Guid>();
        }
        public Guid ProjectId { get; set; }
        public IList<Guid> TaskIds { get; set; }
    }
}
