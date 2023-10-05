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
    public class ContractPaymentService : GenericService<T_PS_CONTRACT_PAYMENT, ContractPaymentRepo>
    {
        public override void Create()
        {
            if(ObjDetail.AMOUNT == 0 && ObjDetail.AMOUNT_ADVANCE == 0 && ObjDetail.INVOICE_VALUE == 0)
            {
                ErrorMessage = "Chưa nhập số tiền.";
                State = false;
                return;
            }
            try
            {
                var contract = UnitOfWork.Repository<ContractRepo>().Get(ObjDetail.CONTRACT_ID);
                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.BeginTransaction();
                this.ObjDetail.UPDATE_DATE = DateTime.Now;
                this.ObjDetail.UPDATE_BY = ProfileUtilities.User?.USER_NAME;
                this.ObjDetail = this.CurrentRepository.Create(this.ObjDetail);
                
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
                foreach(var item in lstId)
                {
                    UnitOfWork.GetSession().Query<T_PS_CONTRACT_PAYMENT>().Where(x => lstId.Contains(x.ID.ToString())).Delete();
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
