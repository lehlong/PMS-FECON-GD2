using NHibernate.Linq;
using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS
{
    public class ProjectResourceOtherService : GenericService<T_PS_RESOURCE_OTHER, ProjectResourceOtherRepo>
    {
        public bool IsSaveOther { get; set; }
        public ProjectResourceOtherService()
        {
        }

        public override void Update()
        {
            var lstItem = this.CurrentRepository.Queryable().Where(x => x.PROJECT_ID == this.ObjDetail.PROJECT_ID).ToList();
            try
            {
                if (this.ObjList.Count(x => string.IsNullOrEmpty(x.FULL_NAME) && x.IS_DELETE != "1") > 0)
                {
                    this.State = false;
                    this.ErrorMessage = "Chưa nhập Họ & tên";
                    return;
                }

                if (this.ObjList.Count(x => string.IsNullOrEmpty(x.VENDOR_CODE) && x.IS_DELETE != "1") > 0)
                {
                    this.State = false;
                    this.ErrorMessage = "Chưa chọn các bên liên quan";
                    return;
                }

                foreach (var item in this.ObjList.Where(x => x.IS_DELETE != "1").ToList())
                {
                    if (item.TO_DATE < item.FROM_DATE)
                    {
                        this.State = false;
                        this.ErrorMessage = "(" + item.FULL_NAME + ") Từ ngày phải nhỏ hơn hoặc bằng đến ngày";
                        return;
                    }
                }

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
                        itemUpdate.VENDOR_CODE = item.VENDOR_CODE;
                        itemUpdate.FULL_NAME = item.FULL_NAME;
                        itemUpdate.CMT = item.CMT;
                        itemUpdate.PHONE = item.PHONE;
                        itemUpdate.EMAIL = item.EMAIL;
                        itemUpdate.FROM_DATE = item.FROM_DATE;
                        itemUpdate.TO_DATE = item.TO_DATE;
                        itemUpdate.VAI_TRO = item.VAI_TRO;
                        this.CurrentRepository.Update(itemUpdate);
                    }
                }
                foreach (var item in this.ObjList.Where(x => x.ID == Guid.Empty && x.IS_DELETE != "1").ToList())
                {
                    item.ID = Guid.NewGuid();
                    item.PROJECT_ID = this.ObjDetail.PROJECT_ID;
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
