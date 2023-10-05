using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class RoleService : GenericService<T_AD_ROLE, RoleRepo>
    {
        public T_AD_USER_GROUP ObjUserGroup { get; set; }
        public List<T_AD_USER_GROUP> ObjListUserGroup { get; set; }
        public RoleService() : base()
        {
            ObjUserGroup = new T_AD_USER_GROUP();
            ObjListUserGroup = new List<T_AD_USER_GROUP>();
        }

        public void SearchUserGroupForAdd()
        {
            this.Get(this.ObjDetail.CODE);
            var lstUserGroup = this.ObjDetail.ListUserGroupRole.Select(x => x.USER_GROUP_CODE).ToList();
            var query = UnitOfWork.Repository<UserGroupRepo>().Queryable();
            query = query.Where(x => !lstUserGroup.Contains(x.CODE));
            if (!string.IsNullOrWhiteSpace(ObjUserGroup.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(ObjUserGroup.CODE.ToLower()) || x.NAME.ToLower().Contains(ObjUserGroup.CODE.ToLower()));
            }
            this.ObjListUserGroup = query.ToList();
        }

        public void AddUserGroupToRole(string lstUserGroup, string roleCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var groupCode in lstUserGroup.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = new T_AD_USER_GROUP_ROLE()
                    {
                        ROLE_CODE = roleCode,
                        USER_GROUP_CODE = groupCode
                    };

                    if (ProfileUtilities.User != null)
                    {
                        item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        item.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                    }
                    UnitOfWork.Repository<UserGroupRoleRepo>().Create(item);
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

        public void DeleteUserGroupOfRole(string lstUserGroup, string roleCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var groupCode in lstUserGroup.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = UnitOfWork.Repository<UserGroupRoleRepo>().Queryable().FirstOrDefault(x => x.ROLE_CODE == roleCode && x.USER_GROUP_CODE == groupCode);
                    if (item != null)
                    {
                        UnitOfWork.Repository<UserGroupRoleRepo>().Delete(item);
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

        public List<NodeRight> BuildTreeRight()
        {
            var lstNode = new List<NodeRight>();
            var listRight = UnitOfWork.Repository<RightRepo>().GetAll().OrderBy(x => x.C_ORDER);
            foreach (var item in listRight)
            {
                var node = new NodeRight()
                {
                    id = item.CODE,
                    pId = item.PARENT,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.NAME
                };
                if (this.ObjDetail.ListRoleDetail.Count(x => x.FK_RIGHT == item.CODE) > 0)
                    node.@checked = "true";
                
                lstNode.Add(node);
            }
            return lstNode;
        }

        public void UpdateRight(string roleCode, string rightList)
        {
            try
            {
                this.Get(roleCode);

                UnitOfWork.BeginTransaction();

                foreach (var item in this.ObjDetail.ListRoleDetail)
                {
                    UnitOfWork.Repository<RoleDetailRepo>().Delete(item);
                }

                foreach (var item in rightList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    UnitOfWork.Repository<RoleDetailRepo>().Create(
                        new T_AD_ROLE_DETAIL()
                        {
                            FK_RIGHT = item,
                            FK_ROLE = roleCode
                        }
                    );
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

        public override void Create()
        {
            try
            {
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
