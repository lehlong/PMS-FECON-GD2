using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using System;
using System.Linq;

namespace SMO.Service.MD
{
    public class NumberRangeService : GenericService<T_MD_NUMBER_RANGE, NumberRangeRepo>
    {
        public NumberRangeService() : base()
        {

        }

        public void UpdateItem()
        {
            foreach (var item in this.ObjList.Where(x => x.IS_DELETE != "1").ToList())
            {
                if (this.ObjList.Count(x => x.CHARACTER == item.CHARACTER) > 1)
                {
                    this.State = false;
                    this.ErrorMessage = "Không được nhập trùng CHARACTER";
                    return;
                }
            }
            var lstItem = this.CurrentRepository.GetAll();
            try
            {
                UnitOfWork.BeginTransaction();
                foreach (var item in this.ObjList.Where(x => x.ID != Guid.Empty).ToList())
                {
                    if (item.IS_DELETE == "1")
                    {
                        var itemDelete = lstItem.FirstOrDefault(x => x.ID == item.ID);
                        this.CurrentRepository.Delete(itemDelete);
                    }
                    else
                    {
                        var itemUpdate = lstItem.FirstOrDefault(x => x.ID == item.ID);
                        itemUpdate.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                        itemUpdate.CHARACTER = item.CHARACTER;
                        itemUpdate.CURRENT_NUMBER = item.CURRENT_NUMBER;
                        this.CurrentRepository.Update(itemUpdate);
                    }
                }
                foreach (var item in this.ObjList.Where(x => x.ID == Guid.Empty && x.IS_DELETE != "1").ToList())
                {
                    item.ID = Guid.NewGuid();
                    item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                    this.CurrentRepository.Create(item);
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
