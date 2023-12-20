using NHibernate.Linq;

using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS
{
    public class ContractRequestPaymentService : GenericService<T_PS_CONTRACT_REQUEST_PAYMENT, ContractRequestPaymentRepo>
    {
        public string WORKFLOW_CODE { get; set; }
        public string WORKFLOW_ID { get; set; }
        public override void Create()
        {
            try
            {
                var contract = UnitOfWork.Repository<ContractRepo>().Get(ObjDetail.CONTRACT_ID);
                var project = UnitOfWork.Repository<ProjectRepo>().Get(contract.PROJECT_ID);
                var workflow = UnitOfWork.Repository<ProjectWorkflowRepo>().Get(Guid.Parse(this.WORKFLOW_ID));

                var documentWorkflowId = Guid.NewGuid();
                var documentId = Guid.NewGuid();

                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.BeginTransaction();

                this.ObjDetail.ID = documentId;
                this.ObjDetail.WORKFLOW_ID = documentWorkflowId;
                this.ObjDetail.STATUS = DocumentWorkflowStatus.KHOI_TAO.GetValue();
                this.ObjDetail.AMOUNT_ADVANCE = this.ObjDetail.AMOUNT * this.ObjDetail.EXCHANGE_RATE;
                this.ObjDetail.UPDATE_DATE = DateTime.Now;
                this.ObjDetail.UPDATE_BY = ProfileUtilities.User?.USER_NAME;
                this.ObjDetail = this.CurrentRepository.Create(this.ObjDetail);

                UnitOfWork.Repository<DocumentWorkflowRepo>().Create(new T_PS_DOCUMENT_WORKFLOW
                {
                    ID = documentWorkflowId,
                    CODE = workflow.CODE,
                    NAME = workflow.NAME,
                    PROJECT_ID = project.ID,
                    REQUEST_TYPE_CODE = workflow.REQUEST_TYPE_CODE,
                    PROJECT_LEVEL_CODE = workflow.PROJECT_LEVEL_CODE,
                    CONTRACT_VALUE_MIN = workflow.CONTRACT_VALUE_MIN,
                    CONTRACT_VALUE_MAX = workflow.CONTRACT_VALUE_MAX,
                    PURCHASE_TYPE_CODE = workflow.PURCHASE_TYPE_CODE,
                    AUTHORITY = workflow.AUTHORITY,
                    DOCUMENT_ID = documentId,
                    UPDATE_BY = ProfileUtilities.User?.USER_NAME,
                    UPDATE_DATE = DateTime.Now,
                    CREATE_BY = ProfileUtilities.User?.USER_NAME,
                    CREATE_DATE = DateTime.Now,
                });

                foreach (var step in workflow.ListSteps)
                {
                    UnitOfWork.Repository<DocumentWorkflowStepRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_STEP
                    {
                        ID = Guid.NewGuid(),
                        WORKFLOW_CODE = step.WORKFLOW_CODE,
                        WORKFLOW_ID = documentWorkflowId,
                        NAME = step.NAME,
                        PROJECT_ROLE_CODE = step.PROJECT_ROLE_CODE,
                        USER_ACTION = step.USER_ACTION,
                        NUMBER_DAYS = step.NUMBER_DAYS,
                        ACTION = step.ACTION,
                        C_ORDER = step.C_ORDER,
                        IS_DONE = false,
                        DOCUMENT_ID = documentId,
                        PROJECT_ID = step.PROJECT_ID,
                        ACTIVE = true,
                        IS_PROCESS = step.C_ORDER == 0 ? true : false,
                        UPDATE_BY = ProfileUtilities.User?.USER_NAME,
                        UPDATE_DATE = DateTime.Now,
                        CREATE_BY = ProfileUtilities.User?.USER_NAME,
                        CREATE_DATE = DateTime.Now,
                    });
                }

                foreach (var step in workflow.ListFiles)
                {
                    UnitOfWork.Repository<DocumentWorkflowFileRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_FILE
                    {
                        ID = Guid.NewGuid(),
                        WORKFLOW_CODE = step.WORKFLOW_CODE,
                        WORKFLOW_ID = documentWorkflowId,
                        NAME = step.NAME,
                        C_ORDER = step.C_ORDER,
                        DOCUMENT_ID = documentId,
                        PROJECT_ID = step.PROJECT_ID,
                        REFERENCE_FILE_ID = Guid.NewGuid(),
                        ACTIVE = true,
                        UPDATE_BY = ProfileUtilities.User?.USER_NAME,
                        UPDATE_DATE = DateTime.Now,
                        CREATE_BY = ProfileUtilities.User?.USER_NAME,
                        CREATE_DATE = DateTime.Now,
                    });
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

        public override void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.BeginTransaction();
                foreach (var item in lstId)
                {
                    UnitOfWork.GetSession().Query<T_PS_CONTRACT_REQUEST_PAYMENT>().Where(x => lstId.Contains(x.ID.ToString())).Delete();
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
        public void UpdateRequestPayment(T_PS_CONTRACT_REQUEST_PAYMENT data)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var item = UnitOfWork.Repository<ContractRequestPaymentRepo>().Queryable().First(x => x.ID == data.ID);
                item.EXPLAIN = data.EXPLAIN;
                item.CONTENTS = data.CONTENTS;
                item.BILL_NUMBER = data.BILL_NUMBER;
                item.CURRENCY_CODE = data.CURRENCY_CODE;
                item.EXCHANGE_RATE = data.EXCHANGE_RATE;
                item.AMOUNT = data.AMOUNT;
                item.AMOUNT_ADVANCE = data.EXCHANGE_RATE * data.AMOUNT;
                item.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                item.UPDATE_DATE = DateTime.Now;
                item.ACCOUNT_NUMBER = data.ACCOUNT_NUMBER;
                item.BANK_NAME = data.BANK_NAME;

                UnitOfWork.Repository<ContractRequestPaymentRepo>().Update(item);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public T_PS_DOCUMENT_WORKFLOW_STEP GetStep(Guid id)
        {
            try
            {
                return UnitOfWork.Repository<DocumentWorkflowStepRepo>().Queryable().First(x => x.ID == id);
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
                return null;
            }
        }

        public T_PS_CONTRACT_REQUEST_PAYMENT GetRequestPayment(Guid id)
        {
            try
            {
                return UnitOfWork.Repository<ContractRequestPaymentRepo>().Queryable().First(x => x.ID == id);
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
                return null;
            }
        }

        public void SaveComment(Guid stepId, string comment)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<DocumentWorkflowCommentRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_COMMENT
                {
                    ID = Guid.NewGuid(),
                    STEP_ID = stepId,
                    COMMENT = comment,
                    CREATE_BY = ProfileUtilities.User.USER_NAME,
                    CREATE_DATE = DateTime.Now,
                });
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        #region Workflow

        public void DocumentTrinhDuyet(Guid documentId)
        {
            try
            {
                var document = UnitOfWork.Repository<ContractRequestPaymentRepo>().Queryable().First(x => x.ID == documentId);
                if (document == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                UnitOfWork.BeginTransaction();

                //Cập nhật trạng thái đề nghị
                document.STATUS = DocumentWorkflowStatus.CHO_PHE_DUYET.GetValue();
                document.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                document.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<ContractRequestPaymentRepo>().Update(document);

                //Cập nhật trạng thái bước hiện tại
                var taskProcessing = document.Workflow.ListSteps.First(x => x.IS_PROCESS);
                if (taskProcessing == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                taskProcessing.SOLVED = "01";
                taskProcessing.IS_DONE = true;
                taskProcessing.IS_PROCESS = false;
                taskProcessing.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                taskProcessing.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<DocumentWorkflowStepRepo>().Update(taskProcessing);

                //Cập nhật bước tiếp theo nếu không phải bước cuối
                if (taskProcessing.C_ORDER + 1 != document.Workflow.ListSteps.Count())
                {
                    var nextProcessing = document.Workflow.ListSteps.First(x => x.C_ORDER == taskProcessing.C_ORDER + 1);
                    if (nextProcessing == null)
                    {
                        ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                        State = false;
                        return;
                    }
                    nextProcessing.IS_PROCESS = true;
                    nextProcessing.IS_DONE = false;
                    UnitOfWork.Repository<DocumentWorkflowStepRepo>().Update(nextProcessing);
                }

                //Lưu lịch sử quy trình
                UnitOfWork.Repository<DocumentWorkflowHistoryRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_HISTORY
                {
                    ID = Guid.NewGuid(),
                    WORKFLOW_CODE = taskProcessing.WORKFLOW_CODE,
                    WORKFLOW_ID = taskProcessing.WORKFLOW_ID,
                    NAME = taskProcessing.NAME,
                    PROJECT_ROLE_CODE = taskProcessing.PROJECT_ROLE_CODE,
                    USER_ACTION = taskProcessing.USER_ACTION,
                    NUMBER_DAYS = taskProcessing.NUMBER_DAYS,
                    ACTION = taskProcessing.ACTION,
                    C_ORDER = taskProcessing.C_ORDER,
                    IS_DONE = taskProcessing.IS_DONE,
                    IS_PROCESS = taskProcessing.IS_PROCESS,
                    ACTIVE = taskProcessing.ACTIVE,
                    SOLVED = "01",
                    DOCUMENT_ID = taskProcessing.DOCUMENT_ID,
                    PROJECT_ID = taskProcessing.PROJECT_ID,
                    CREATE_BY = ProfileUtilities.User.USER_NAME,
                    CREATE_DATE = DateTime.Now,
                });

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void DocumentPheDuyet(Guid documentId)
        {
            try
            {
                var document = UnitOfWork.Repository<ContractRequestPaymentRepo>().Queryable().First(x => x.ID == documentId);
                if (document == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                UnitOfWork.BeginTransaction();

                //Cập nhật trạng thái đề nghị
                document.STATUS = DocumentWorkflowStatus.DA_PHE_DUYET.GetValue();
                document.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                document.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<ContractRequestPaymentRepo>().Update(document);

                //Cập nhật trạng thái bước hiện tại
                var taskProcessing = document.Workflow.ListSteps.First(x => x.IS_PROCESS);
                if (taskProcessing == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                taskProcessing.SOLVED = DocumentWorkflowStatus.DA_PHE_DUYET.GetValue();
                taskProcessing.IS_DONE = true;
                taskProcessing.IS_PROCESS = false;
                taskProcessing.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                taskProcessing.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<DocumentWorkflowStepRepo>().Update(taskProcessing);

                //Cập nhật bước tiếp theo nếu không phải bước cuối
                if (taskProcessing.C_ORDER + 1 != document.Workflow.ListSteps.Count())
                {
                    var nextProcessing = document.Workflow.ListSteps.First(x => x.C_ORDER == taskProcessing.C_ORDER + 1);
                    if (nextProcessing == null)
                    {
                        ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                        State = false;
                        return;
                    }
                    nextProcessing.IS_PROCESS = true;
                    nextProcessing.IS_DONE = false;
                    UnitOfWork.Repository<DocumentWorkflowStepRepo>().Update(nextProcessing);
                }

                //Lưu lịch sử quy trình
                UnitOfWork.Repository<DocumentWorkflowHistoryRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_HISTORY
                {
                    ID = Guid.NewGuid(),
                    WORKFLOW_CODE = taskProcessing.WORKFLOW_CODE,
                    WORKFLOW_ID = taskProcessing.WORKFLOW_ID,
                    NAME = taskProcessing.NAME,
                    PROJECT_ROLE_CODE = taskProcessing.PROJECT_ROLE_CODE,
                    USER_ACTION = taskProcessing.USER_ACTION,
                    NUMBER_DAYS = taskProcessing.NUMBER_DAYS,
                    ACTION = taskProcessing.ACTION,
                    C_ORDER = taskProcessing.C_ORDER,
                    IS_DONE = taskProcessing.IS_DONE,
                    IS_PROCESS = taskProcessing.IS_PROCESS,
                    ACTIVE = taskProcessing.ACTIVE,
                    SOLVED = "03",
                    DOCUMENT_ID = taskProcessing.DOCUMENT_ID,
                    PROJECT_ID = taskProcessing.PROJECT_ID,
                    CREATE_BY = ProfileUtilities.User.USER_NAME,
                    CREATE_DATE = DateTime.Now,
                });

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
        public void DocumentYeuCauChinhSua(Guid documentId)
        {
            try
            {
                var document = UnitOfWork.Repository<ContractRequestPaymentRepo>().Queryable().First(x => x.ID == documentId);
                if (document == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                UnitOfWork.BeginTransaction();

                //Cập nhật trạng thái đề nghị
                document.STATUS = DocumentWorkflowStatus.KHOI_TAO.GetValue();
                document.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                document.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<ContractRequestPaymentRepo>().Update(document);

                //Lưu lịch sử
                var taskProcessing = document.Workflow.ListSteps.First(x => x.IS_PROCESS);
                UnitOfWork.Repository<DocumentWorkflowHistoryRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_HISTORY
                {
                    ID = Guid.NewGuid(),
                    WORKFLOW_CODE = taskProcessing.WORKFLOW_CODE,
                    WORKFLOW_ID = taskProcessing.WORKFLOW_ID,
                    NAME = taskProcessing.NAME,
                    PROJECT_ROLE_CODE = taskProcessing.PROJECT_ROLE_CODE,
                    USER_ACTION = taskProcessing.USER_ACTION,
                    NUMBER_DAYS = taskProcessing.NUMBER_DAYS,
                    ACTION = taskProcessing.ACTION,
                    C_ORDER = taskProcessing.C_ORDER,
                    IS_DONE = taskProcessing.IS_DONE,
                    IS_PROCESS = taskProcessing.IS_PROCESS,
                    ACTIVE = taskProcessing.ACTIVE,
                    SOLVED = "02",
                    DOCUMENT_ID = taskProcessing.DOCUMENT_ID,
                    PROJECT_ID = taskProcessing.PROJECT_ID,
                    CREATE_BY = ProfileUtilities.User.USER_NAME,
                    CREATE_DATE = DateTime.Now,
                });

                //Reset quy trình
                foreach (var step in document.Workflow.ListSteps)
                {
                    step.IS_DONE = false;
                    step.SOLVED = null;
                    step.IS_PROCESS = step.C_ORDER == 0 ? true : false;
                    step.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    step.UPDATE_DATE = DateTime.Now;
                    UnitOfWork.Repository<DocumentWorkflowStepRepo>().Update(step);
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

        public void DocumentXacNhan(Guid documentId)
        {
            try
            {
                var document = UnitOfWork.Repository<ContractRequestPaymentRepo>().Queryable().First(x => x.ID == documentId);
                if (document == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                UnitOfWork.BeginTransaction();

                //Cập nhật trạng thái đề nghị
                document.STATUS = DocumentWorkflowStatus.DA_XAC_NHAN.GetValue();
                document.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                document.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<ContractRequestPaymentRepo>().Update(document);

                //Cập nhật trạng thái bước hiện tại
                var taskProcessing = document.Workflow.ListSteps.First(x => x.IS_PROCESS);
                if (taskProcessing == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                taskProcessing.SOLVED = DocumentWorkflowStatus.DA_XAC_NHAN.GetValue();
                taskProcessing.IS_DONE = true;
                taskProcessing.IS_PROCESS = false;
                taskProcessing.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                taskProcessing.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<DocumentWorkflowStepRepo>().Update(taskProcessing);

                //Cập nhật bước tiếp theo nếu không phải bước cuối
                if (taskProcessing.C_ORDER + 1 != document.Workflow.ListSteps.Count())
                {
                    var nextProcessing = document.Workflow.ListSteps.First(x => x.C_ORDER == taskProcessing.C_ORDER + 1);
                    if (nextProcessing == null)
                    {
                        ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                        State = false;
                        return;
                    }
                    nextProcessing.IS_PROCESS = true;
                    nextProcessing.IS_DONE = false;
                    UnitOfWork.Repository<DocumentWorkflowStepRepo>().Update(nextProcessing);
                }

                //Lưu lịch sử quy trình
                UnitOfWork.Repository<DocumentWorkflowHistoryRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_HISTORY
                {
                    ID = Guid.NewGuid(),
                    WORKFLOW_CODE = taskProcessing.WORKFLOW_CODE,
                    WORKFLOW_ID = taskProcessing.WORKFLOW_ID,
                    NAME = taskProcessing.NAME,
                    PROJECT_ROLE_CODE = taskProcessing.PROJECT_ROLE_CODE,
                    USER_ACTION = taskProcessing.USER_ACTION,
                    NUMBER_DAYS = taskProcessing.NUMBER_DAYS,
                    ACTION = taskProcessing.ACTION,
                    C_ORDER = taskProcessing.C_ORDER,
                    IS_DONE = taskProcessing.IS_DONE,
                    IS_PROCESS = taskProcessing.IS_PROCESS,
                    ACTIVE = taskProcessing.ACTIVE,
                    SOLVED = "05",
                    DOCUMENT_ID = taskProcessing.DOCUMENT_ID,
                    PROJECT_ID = taskProcessing.PROJECT_ID,
                    CREATE_BY = ProfileUtilities.User.USER_NAME,
                    CREATE_DATE = DateTime.Now,
                });

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void DocumentTuChoi(Guid documentId)
        {
            try
            {
                var document = UnitOfWork.Repository<ContractRequestPaymentRepo>().Queryable().First(x => x.ID == documentId);
                if (document == null)
                {
                    ErrorMessage = "Lỗi không thể thực hiện được thao tác này! Vui lòng liên hệ với quản trị viên hệ thống!";
                    State = false;
                    return;
                }
                UnitOfWork.BeginTransaction();

                //Cập nhật trạng thái đề nghị
                document.STATUS = DocumentWorkflowStatus.TU_CHOI.GetValue();
                document.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                document.UPDATE_DATE = DateTime.Now;
                UnitOfWork.Repository<ContractRequestPaymentRepo>().Update(document);

                var taskProcessing = document.Workflow.ListSteps.First(x => x.IS_PROCESS);
                //Lưu lịch sử quy trình
                UnitOfWork.Repository<DocumentWorkflowHistoryRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_HISTORY
                {
                    ID = Guid.NewGuid(),
                    WORKFLOW_CODE = taskProcessing.WORKFLOW_CODE,
                    WORKFLOW_ID = taskProcessing.WORKFLOW_ID,
                    NAME = taskProcessing.NAME,
                    PROJECT_ROLE_CODE = taskProcessing.PROJECT_ROLE_CODE,
                    USER_ACTION = taskProcessing.USER_ACTION,
                    NUMBER_DAYS = taskProcessing.NUMBER_DAYS,
                    ACTION = taskProcessing.ACTION,
                    C_ORDER = taskProcessing.C_ORDER,
                    IS_DONE = taskProcessing.IS_DONE,
                    IS_PROCESS = taskProcessing.IS_PROCESS,
                    ACTIVE = taskProcessing.ACTIVE,
                    SOLVED = "04",
                    DOCUMENT_ID = taskProcessing.DOCUMENT_ID,
                    PROJECT_ID = taskProcessing.PROJECT_ID,
                    CREATE_BY = ProfileUtilities.User.USER_NAME,
                    CREATE_DATE = DateTime.Now,
                });

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
        #endregion
    }
}
