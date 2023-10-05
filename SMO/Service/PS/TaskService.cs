using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;

using System;

namespace SMO.Service.PS
{
    public class TaskService : GenericService<T_PS_TASK, TaskRepo>
    {
        internal void GetDetail(Guid id)
        {
            ObjDetail = UnitOfWork.Repository<TaskRepo>().GetDetail(id);
        }
    }
}
