using NHibernate.Linq;

using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Service.PS
{
    public class ProjectResourceService : GenericService<T_PS_RESOURCE, ProjectResourceRepo>
    {
        public IList<T_AD_USER> Users { get; set; }
        public T_AD_USER ObjUserFilter { get; set; }
        public bool IsSaveRole { get; set; }

        public ProjectResourceService()
        {
            ObjUserFilter = new T_AD_USER();
            Users = new List<T_AD_USER>();
        }
        internal void SearchUser()
        {
            try
            {
                int total;
                var assignedUserName = ObjList.Select(x => x.USER_NAME).ToList();
                Users = UnitOfWork.Repository<ProjectResourceRepo>().SearchUser(ObjUserFilter, this.NumerRecordPerPage, this.Page, out total).ToList();
                Users = Users.Where(x => !assignedUserName.Contains(x.USER_NAME)).ToList();
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void AddResources(Guid projectId, string strLstSelected)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var lstUserNames = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                UnitOfWork.BeginTransaction();
                CurrentRepository.Create((from username in lstUserNames
                                          select new T_PS_RESOURCE
                                          {
                                              ID = Guid.NewGuid(),
                                              PROJECT_ID = projectId,
                                              USER_NAME = username,
                                              CREATE_BY = currentUser,
                                          }).ToList());
                //UnitOfWork.Repository<ProjectRepo>().ResetStatus(projectId, currentUser,"Nhân sự dự án");
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public List<NodeRight> BuildTreeRight(Guid projectId, string userName)
        {
            var lstNode = new List<NodeRight>();

            var lstAllRight = UnitOfWork.GetSession().Query<T_AD_PROJECT_RIGHT>().OrderBy(x => x.C_ORDER).ToList();
            lstAllRight.Add(new T_AD_PROJECT_RIGHT() { CODE = "R00", NAME = "Tất cả quyền trong dự án" });
            var lstRightOfUser = UnitOfWork.GetSession().Query<T_PS_PROJECT_USER_RIGHT>().Where(x => x.USER_NAME == userName && x.PROJECT_ID == projectId).ToList();
            var lstRoleIdOfUser = UnitOfWork.GetSession().Query<T_PS_RESOURCE>().FirstOrDefault(x => x.USER_NAME == userName && x.PROJECT_ID == projectId)?.PROJECT_ROLE_ID?.Split(',').ToList();
            if (lstRoleIdOfUser == null)
            {
                lstRoleIdOfUser = new List<string>();
            }
            var lstRightOfRole = UnitOfWork.GetSession().Query<T_MD_PROJECT_ROLE_RIGHT>().Where(x => lstRoleIdOfUser.Contains(x.PROJECT_ROLE_ID)).ToList();

            foreach (var item in lstAllRight)
            {
                var node = new NodeRight()
                {
                    id = item.CODE,
                    pId = item.PARENT,
                    name = "<span class='spMaOnTree'>" + item.CODE + "</span>" + item.NAME
                };
                if (lstRightOfRole.Count(x => x.RIGHT_CODE == item.CODE) > 0)
                    node.@checked = "true";

                if (string.IsNullOrWhiteSpace(item.PARENT))
                {
                    node.pId = "R00";
                }
                lstNode.Add(node);
            }

            //Kiểm tra các quyền đặc biệt
            foreach (var item in lstRightOfUser)
            {
                if (item.IS_ADD && lstRightOfRole.Count(x => x.RIGHT_CODE == item.RIGHT_CODE) == 0)
                {
                    var node = lstNode.Where(x => x.id == item.RIGHT_CODE).FirstOrDefault();
                    node.@checked = "true";
                    node.isAdd = "1";
                }

                if (item.IS_REMOVE && lstRightOfRole.Count(x => x.RIGHT_CODE == item.RIGHT_CODE) >= 0)
                {
                    var node = lstNode.Where(x => x.id == item.RIGHT_CODE).FirstOrDefault();
                    node.@checked = "false";
                    node.isRemove = "1";
                }
            }

            return lstNode;
        }

        public void UpdateRight(string userName, Guid projectId, string rightList, string statusList)
        {
            try
            {
                var lstRightOfUser = UnitOfWork.GetSession().Query<T_PS_PROJECT_USER_RIGHT>().Where(x => x.USER_NAME == userName && x.PROJECT_ID == projectId).ToList();
                var lstRoleIdOfUser = UnitOfWork.GetSession().Query<T_PS_RESOURCE>().FirstOrDefault(x => x.USER_NAME == userName && x.PROJECT_ID == projectId)?.PROJECT_ROLE_ID?.Split(',').ToList();
                if (lstRoleIdOfUser == null)
                {
                    lstRoleIdOfUser = new List<string>();
                }
                var lstRightOfRole = UnitOfWork.GetSession().Query<T_MD_PROJECT_ROLE_RIGHT>().Where(x => lstRoleIdOfUser.Contains(x.PROJECT_ROLE_ID)).ToList();

                var lstStatus = statusList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var lstRight = rightList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

                UnitOfWork.BeginTransaction();

                for (int i = 0; i < lstRight.Length; i++)
                {
                    var right = lstRight[i];
                    if (lstRightOfUser.Count(x => x.RIGHT_CODE == right) > 0)
                    {
                        var righModify = lstRightOfUser.FirstOrDefault(x => x.RIGHT_CODE == right);
                        lstRightOfUser.Remove(righModify);
                        UnitOfWork.GetSession().Delete(righModify);
                    }

                    if (lstStatus[i] == "true")
                    {
                        if (lstRightOfRole.Count(x => x.RIGHT_CODE == right) == 0 && lstRightOfUser.Count(x => x.RIGHT_CODE == right) == 0)
                        {
                            UnitOfWork.GetSession().Save(
                                new T_PS_PROJECT_USER_RIGHT()
                                {
                                    ID = Guid.NewGuid(),
                                    PROJECT_ID = projectId,
                                    RIGHT_CODE = right,
                                    USER_NAME = userName,
                                    IS_ADD = true,
                                    IS_REMOVE = false
                                }
                            );
                        }
                    }
                    else if (lstStatus[i] == "false")
                    {
                        if (lstRightOfRole.Count(x => x.RIGHT_CODE == right) >= 0 && lstRightOfUser.Count(x => x.RIGHT_CODE == right) == 0)
                        {
                            UnitOfWork.GetSession().Save(
                                new T_PS_PROJECT_USER_RIGHT()
                                {
                                    ID = Guid.NewGuid(),
                                    PROJECT_ID = projectId,
                                    RIGHT_CODE = right,
                                    USER_NAME = userName,
                                    IS_ADD = false,
                                    IS_REMOVE = true
                                }
                            );
                        }
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

        public override void Delete(string strLstSelected)
        {

            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var projectId = CurrentRepository.Get(new Guid(lstId.First()))?.PROJECT_ID;
                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.BeginTransaction();

                this.CurrentRepository.Delete(lstId: (from id in lstId
                                                      select new Guid(id)).Cast<object>().ToList());
                //if (projectId.HasValue)
                //{
                //    UnitOfWork.Repository<ProjectRepo>().ResetStatus(projectId.Value, currentUser, "Nhân sự dự án");
                //}
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
            try
            {
                UnitOfWork.BeginTransaction();

                if (this.ObjList.Count(x => string.IsNullOrEmpty(x.PROJECT_ROLE_ID) || string.IsNullOrEmpty(x.PROJECT_USER_TYPE_CODE) || string.IsNullOrEmpty(x.FROM_DATE?.ToString("dd/MM/yyyy")) || string.IsNullOrEmpty(x.TO_DATE?.ToString("dd/MM/yyyy"))) > 0)
                {
                    this.State = false;
                    this.ErrorMessage = "Chưa nhập đủ thông tin vai trò, loại nhân sự, từ ngày, đến ngày!";
                    return;
                }

                if (this.ObjList.Count(x => x.FROM_DATE > x.TO_DATE) > 0)
                {
                    foreach (var item in this.ObjList.Where(x => x.FROM_DATE > x.TO_DATE))
                    {
                        if (item.TO_DATE < item.FROM_DATE)
                        {
                            this.State = false;
                            this.ErrorMessage = "(" + item.USER_NAME + ") Từ ngày phải nhỏ hơn hoặc bằng đến ngày";
                            return;
                        }
                    }
                }
                var currentUsername = ProfileUtilities.User?.USER_NAME;

                foreach (var item in ObjList)
                {
                    item.UPDATE_BY = currentUsername;
                    CurrentRepository.Update(item);
                }
                //UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, currentUsername, "Nhân sự dự án");
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
