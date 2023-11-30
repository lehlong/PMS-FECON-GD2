using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using System;

namespace SMO.Service.MD
{
    public class WorkflowService : GenericService<T_MD_WORKFLOW, WorkflowRepo>
    {
        public WorkflowService() : base()
        {

        }

        public override void Create()
        {
            try
            {
                this.ObjDetail.ACTIVE = true;
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE))
                {
                    base.Create();
                }
                else
                {
                    this.State = false;
                    this.MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
