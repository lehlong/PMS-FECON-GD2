using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.AD
{
    public class UserGroupService : GenericService<T_AD_USER_GROUP, UserGroupRepo>
    {
        public T_AD_USER ObjUser { get; set; }
        public List<T_AD_USER> ObjListUser { get; set; }
        public T_AD_ROLE ObjRole { get; set; }
        public List<T_AD_ROLE> ObjListRole { get; set; }
        public UserGroupService() : base()
        {
            ObjUser = new T_AD_USER();
            ObjListUser = new List<T_AD_USER>();
            ObjRole = new T_AD_ROLE();
            ObjListRole = new List<T_AD_ROLE>();
        }

        public void SearchUserForAdd()
        {
            this.Get(this.ObjDetail.CODE);
            var lstUserOfGroup = this.ObjDetail.ListUserUserGroup.Select(x => x.USER_NAME).ToList();
            var query = UnitOfWork.Repository<UserRepo>().Queryable();
            query = query.Where(x => !lstUserOfGroup.Contains(x.USER_NAME));
            if (!string.IsNullOrWhiteSpace(ObjUser.USER_NAME))
            {
                query = query.Where(x => x.USER_NAME.ToLower().Contains(ObjUser.USER_NAME.ToLower()) || x.FULL_NAME.ToLower().Contains(ObjUser.USER_NAME.ToLower()));
            }
            this.ObjListUser = query.ToList();
        }

        public void SearchRoleForAdd()
        {
            this.Get(this.ObjDetail.CODE);
            var lstRoleOfUserGroup = this.ObjDetail.ListUserGroupRole.Select(x => x.ROLE_CODE).ToList();
            var query = UnitOfWork.Repository<RoleRepo>().Queryable();
            query = query.Where(x => !lstRoleOfUserGroup.Contains(x.CODE));
            if (!string.IsNullOrWhiteSpace(ObjRole.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(ObjRole.CODE.ToLower()) || x.NAME.ToLower().Contains(ObjRole.CODE.ToLower()));
            }
            this.ObjListRole = query.ToList();
        }

        public List<NodeRight> BuildTreeRight(string userGroupCode)
        {
            var lstNode = new List<NodeRight>();
            this.Get(userGroupCode);

            //Danh sách tất cả các quyền
            var lstAllRight = UnitOfWork.Repository<RightRepo>().GetAll().OrderBy(x => x.C_ORDER);

            //Danh sách role của user theo usergroup
            var lstRole = new List<T_AD_ROLE>();
            foreach (var item1 in this.ObjDetail.ListUserUserGroup)
            {
                foreach (var item2 in item1.UserGroup.ListUserGroupRole)
                {
                    lstRole.Add(item2.Role);
                }
            }
            lstRole = lstRole.Distinct().ToList();

            //Danh sách các quyền của tập hợp role trên
            var lstRoleDetail = new List<T_AD_ROLE_DETAIL>();
            foreach (var item in lstRole)
            {
                lstRoleDetail.AddRange(item.ListRoleDetail);
            }
            lstRoleDetail = lstRoleDetail.Distinct().ToList();

            foreach (var item in lstAllRight)
            {
                var node = new NodeRight()
                {
                    id = item.CODE,
                    pId = item.PARENT,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.NAME
                };
                if (lstRoleDetail.Count(x => x.FK_RIGHT == item.CODE) > 0)
                    node.@checked = "true";

                lstNode.Add(node);
            }

            return lstNode;
        }

        public void AddUserToGroup(string lstUser, string userGroupCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var userName in lstUser.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = new T_AD_USER_USER_GROUP()
                    {
                        USER_NAME = userName,
                        USER_GROUP_CODE = userGroupCode
                    };

                    if (ProfileUtilities.User != null)
                    {
                        item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        item.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                    }
                    UnitOfWork.Repository<UserUserGroupRepo>().Create(item);
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

        public void AddRoleToUserGroup(string lstRole, string userGroupCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var roleCode in lstRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = new T_AD_USER_GROUP_ROLE()
                    {
                        ROLE_CODE = roleCode,
                        USER_GROUP_CODE = userGroupCode
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

        public void DeleteUserOfGroup(string lstUser, string userGroupCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var userName in lstUser.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = UnitOfWork.Repository<UserUserGroupRepo>().Queryable().FirstOrDefault(x => x.USER_NAME == userName && x.USER_GROUP_CODE == userGroupCode);
                    if (item != null)
                    {
                        UnitOfWork.Repository<UserUserGroupRepo>().Delete(item);
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

        public void DeleteRoleOfUserGroup(string lstRole, string userGroupCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var roleCode in lstRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = UnitOfWork.Repository<UserGroupRoleRepo>().Queryable().FirstOrDefault(x => x.ROLE_CODE == roleCode && x.USER_GROUP_CODE == userGroupCode);
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
                    this.MesseageCode = "1105";
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
