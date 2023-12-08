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
        public override void Create()
        {
            try
            {
                var contract = UnitOfWork.Repository<ContractRepo>().Get(ObjDetail.CONTRACT_ID);
                var project = UnitOfWork.Repository<ProjectRepo>().Get(contract.PROJECT_ID);
                var workflow = UnitOfWork.Repository<ProjectWorkflowRepo>().Get(this.WORKFLOW_CODE);

                var documentWorkflowId = Guid.NewGuid();
                var documentId = Guid.NewGuid();

                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.BeginTransaction();

                this.ObjDetail.ID = documentId;
                this.ObjDetail.WORKFLOW_ID = documentWorkflowId;
                this.ObjDetail.STATUS = "01";
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
                });

                foreach(var step in workflow.ListSteps)
                {
                    UnitOfWork.Repository<DocumentWorkflowStepRepo>().Create(new T_PS_DOCUMENT_WORKFLOW_STEP
                    {
                        ID= Guid.NewGuid(),
                        WORKFLOW_CODE = step.WORKFLOW_CODE,
                        WORKFLOW_ID = documentWorkflowId,
                        NAME = step.NAME,
                        PROJECT_ROLE_CODE = step.PROJECT_ROLE_CODE,
                        USER_ACTION = step.USER_ACTION,
                        NUMBER_DAYS = step.NUMBER_DAYS,
                        ACTION = step.ACTION,
                        C_ORDER =step.C_ORDER,
                        IS_DONE = false,
                        DOCUMENT_ID = documentId,
                        PROJECT_ID = step.PROJECT_ID,
                        ACTIVE = true,
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
                        ACTIVE = true,
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
        public override void Update()
        {
            if (ObjDetail.AMOUNT == 0 && ObjDetail.AMOUNT_ADVANCE == 0 && ObjDetail.INVOICE_VALUE == 0)
            {
                ErrorMessage = "Chưa nhập số tiền.";
                State = false;
                return;
            }
            try
            {
                UnitOfWork.BeginTransaction();

                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    //this.ObjDetail.UPDATE_DATE = this.CurrentRepository.GetDateDatabase();
                }

                this.ObjDetail = this.CurrentRepository.Update(this.ObjDetail);
                var contract = UnitOfWork.Repository<ContractRepo>().Get(ObjDetail.CONTRACT_ID);
                var currentUser = ProfileUtilities.User?.USER_NAME;

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
