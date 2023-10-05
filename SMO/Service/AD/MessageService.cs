using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class MessageService : GenericService<T_AD_MESSAGE, MessageRepo>
    {
        public MessageService() : base()
        {

        }

        public void Update(string message, string id)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                this.Get(id);
                this.ObjDetail.MESSAGE = message.Trim();
                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    this.ObjDetail.UPDATE_DATE = this.CurrentRepository.GetDateDatabase();
                }
                this.CurrentRepository.Update(this.ObjDetail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public override void Create()
        {
            try
            {
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE && x.LANGUAGE == this.ObjDetail.LANGUAGE))
                {
                    this.ObjDetail.PKID = Guid.NewGuid().ToString();
                    this.ObjDetail.ACTIVE = true;
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
