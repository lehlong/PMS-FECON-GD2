using NHibernate.Linq;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;
using System;
using System.Linq;

namespace SMO.Service.PS
{
    public class ProjectWorkflowService : GenericService<T_PS_PROJECT_WORKFLOW, ProjectWorkflowRepo>
    {
        public ProjectWorkflowService() : base()
        {

        }

        public void GenerateProjectWorkflow(Guid projectId)
        {
            try
            {
                var project = UnitOfWork.Repository<ProjectRepo>().Queryable().FirstOrDefault(x => x.ID == projectId);
                if (project == null)
                {
                    this.State = false;
                    this.ErrorMessage = "Đã có lỗi xảy ra! Vui lòng liên hệ với quản trị hệ thống để xử lý";
                    return;
                }
                var lstTemplateWorkflow = UnitOfWork.Repository<WorkflowRepo>().Queryable().Where(x => x.ACTIVE == true && x.PROJECT_LEVEL_CODE == project.PROJECT_LEVEL_CODE).ToList();

                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<ProjectWorkflowRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).Delete();
                UnitOfWork.Repository<ProjectWorkflowStepRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).Delete();
                UnitOfWork.Repository<ProjectWorkflowFileRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).Delete();
                foreach (var wf in lstTemplateWorkflow)
                {
                    UnitOfWork.Repository<ProjectWorkflowRepo>().Create(new T_PS_PROJECT_WORKFLOW
                    {
                        CODE = wf.CODE,
                        NAME = wf.NAME,
                        PROJECT_ID = projectId,
                        ACTIVE = false,
                        REQUEST_TYPE_CODE = wf.REQUEST_TYPE_CODE,
                        PROJECT_LEVEL_CODE = wf.PROJECT_LEVEL_CODE,
                        CONTRACT_VALUE_MAX = wf.CONTRACT_VALUE_MAX,
                        CONTRACT_VALUE_MIN = wf.CONTRACT_VALUE_MIN,
                        PURCHASE_TYPE_CODE = wf.PURCHASE_TYPE_CODE,
                        AUTHORITY = wf.AUTHORITY,
                    });

                    foreach (var step in wf.ListSteps)
                    {
                        UnitOfWork.Repository<ProjectWorkflowStepRepo>().Create(new T_PS_PROJECT_WORKFLOW_STEP
                        {
                            ID = step.ID,
                            WORKFLOW_CODE = step.WORKFLOW_CODE,
                            NAME = step.NAME,
                            ACTIVE = step.ACTIVE,
                            PROJECT_ROLE_CODE = step.PROJECT_ROLE_CODE,
                            USER_ACTION = step.USER_ACTION,
                            NUMBER_DAYS = step.NUMBER_DAYS,
                            ACTION = step.ACTION,
                            PROJECT_ID = projectId,
                            C_ORDER = step.C_ORDER,
                        });
                    }

                    foreach (var file in wf.ListFiles)
                    {
                        UnitOfWork.Repository<ProjectWorkflowFileRepo>().Create(new T_PS_PROJECT_WORKFLOW_FILE
                        {
                            ID = file.ID,
                            WORKFLOW_CODE = file.WORKFLOW_CODE,
                            NAME = file.NAME,
                            ACTIVE = file.ACTIVE,
                            PROJECT_ID = projectId,
                            C_ORDER = file.C_ORDER,
                        });
                    }
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
