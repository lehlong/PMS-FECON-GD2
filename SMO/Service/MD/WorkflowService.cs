using NHibernate.Linq;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.MD
{
    public class WorkflowService : GenericService<T_MD_WORKFLOW, WorkflowRepo>
    {
        public WorkflowService() : base()
        {

        }

        public void CreateData(T_MD_WORKFLOW workflow, IList<T_MD_WORKFLOW_STEP> workflowStep, IList<T_MD_WORKFLOW_FILE> workflowFile)
        {
            try
            {
                if (UnitOfWork.Repository<WorkflowRepo>().CheckExist(x => x.CODE == this.ObjDetail.CODE))
                {
                    this.State = false;
                    this.ErrorMessage = $"Mã WORKFLOW {workflow.CODE} bị trùng! Vui lòng kiểm tra lại!";
                    return;
                }
                if (workflow.CONTRACT_VALUE_MIN > workflow.CONTRACT_VALUE_MAX)
                {
                    this.State = false;
                    this.ErrorMessage = $"Giá trị hợp đồng MIN > Giá trị hợp đồng MAX! Vui lòng kiểm tra lại!";
                    return;
                }

                workflow.ACTIVE = true;
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<WorkflowRepo>().Create(workflow);

                foreach(var item in workflowStep)
                {
                    if(string.IsNullOrEmpty(item.USER_ACTION) && string.IsNullOrEmpty(item.PROJECT_ROLE_CODE))
                    {
                        this.State = false;
                        this.ErrorMessage = $"Bắt buộc nhập một trong hai trường thông tin Vai trò xử lý hoặc Người xử lý! Vui lòng kiểm tra lại tại bước: {item.NAME}!";
                        return;
                    }
                    item.ID = Guid.NewGuid();
                    item.WORKFLOW_CODE = workflow.CODE;
                    item.ACTIVE = true;
                    UnitOfWork.Repository<WorkflowStepRepo>().Create(item);
                }
                foreach (var item in workflowFile)
                {
                    item.ID = Guid.NewGuid();
                    item.WORKFLOW_CODE = workflow.CODE;
                    item.ACTIVE = true;
                    UnitOfWork.Repository<WorkflowFileRepo>().Create(item);
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

        public void UpdateData(T_MD_WORKFLOW workflow, IList<T_MD_WORKFLOW_STEP> workflowStep, IList<T_MD_WORKFLOW_FILE> workflowFile)
        {
            try
            {
                if (workflow.CONTRACT_VALUE_MIN > workflow.CONTRACT_VALUE_MAX)
                {
                    this.State = false;
                    this.ErrorMessage = $"Giá trị hợp đồng MIN > Giá trị hợp đồng MAX! Vui lòng kiểm tra lại!";
                    return;
                }

                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<WorkflowRepo>().Update(workflow);

                foreach (var item in workflowStep)
                {
                    if (string.IsNullOrEmpty(item.USER_ACTION) && string.IsNullOrEmpty(item.PROJECT_ROLE_CODE))
                    {
                        this.State = false;
                        this.ErrorMessage = $"Bắt buộc nhập một trong hai trường thông tin Vai trò xử lý hoặc Người xử lý! Vui lòng kiểm tra lại tại bước: {item.NAME}!";
                        return;
                    }

                    var step = UnitOfWork.Repository<WorkflowStepRepo>().Queryable().FirstOrDefault(x => x.ID == item.ID);
                    if(step == null)
                    {
                        item.ID = Guid.NewGuid();
                        item.WORKFLOW_CODE = workflow.CODE;
                        item.ACTIVE = true;
                        UnitOfWork.Repository<WorkflowStepRepo>().Create(item);
                    }
                    else
                    {
                        step.ACTION = item.ACTION;
                        step.NAME = item.NAME;
                        step.PROJECT_ROLE_CODE = item.PROJECT_ROLE_CODE;
                        step.USER_ACTION = item.USER_ACTION;
                        step.NUMBER_DAYS = item.NUMBER_DAYS;
                        UnitOfWork.Repository<WorkflowStepRepo>().Update(step);
                    }
                    
                }
                foreach (var item in workflowFile)
                {
                    var file = UnitOfWork.Repository<WorkflowFileRepo>().Queryable().FirstOrDefault(x => x.ID == item.ID);
                    if (file == null)
                    {
                        item.ID = Guid.NewGuid();
                        item.WORKFLOW_CODE = workflow.CODE;
                        item.ACTIVE = true;
                        UnitOfWork.Repository<WorkflowFileRepo>().Create(item);
                    }
                    else
                    {
                        file.NAME = item.NAME;
                        UnitOfWork.Repository<WorkflowFileRepo>().Update(file);
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

        public void DeleteStep(Guid id)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<WorkflowStepRepo>().Queryable().Where(x => x.ID == id).Delete();
                UnitOfWork.Commit();
            }
            catch(Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void DeleteFile(Guid id)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<WorkflowFileRepo>().Queryable().Where(x => x.ID == id).Delete();
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
