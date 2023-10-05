using NHibernate.Linq;
using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;

namespace SMO.Service.AD
{
    public class UserService : GenericService<T_AD_USER, UserRepo>
    {
        internal string GetTicket()
        {
            var ticket = string.Empty;
            try
            {
                using (var webClient = new WebClient())
                {

                    // Get the Trusted Ticket
                    webClient.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded;charset=UTF-8");

                    ticket = webClient.UploadString(
                        string.Format("{0}/trusted", "https://pmsbi.fecon.com.vn"),
                        string.Format("username={0}", "d2s"));
                }
            }
            catch (Exception ex)
            {
                ticket = ex.ToString();
            }
            
            return ticket;
        }
        public T_AD_USER_GROUP ObjUserGroup { get; set; }
        public List<T_AD_USER_GROUP> ObjListUserGroup { get; set; }
        public T_AD_ROLE ObjRole { get; set; }
        public T_MD_CUSTOMER_OLD ObjCustomer { get; set; }
        public T_MD_VENDOR_OLD ObjVendor { get; set; }
        public List<T_AD_ROLE> ObjListRole { get; set; }
        public List<T_MD_VENDOR_OLD> ObjListVendor { get; set; }
        public List<T_MD_CUSTOMER_OLD> ObjListCustomer { get; set; }
        public string COMPANY_ID { get; set; }
        public UserService() : base()
        {
            ObjUserGroup = new T_AD_USER_GROUP();
            ObjListUserGroup = new List<T_AD_USER_GROUP>();
            ObjRole = new T_AD_ROLE();
            ObjVendor = new T_MD_VENDOR_OLD();
            ObjListRole = new List<T_AD_ROLE>();
            ObjCustomer = new T_MD_CUSTOMER_OLD();
            ObjListCustomer = new List<T_MD_CUSTOMER_OLD>();
            ObjListVendor = new List<T_MD_VENDOR_OLD>();
        }

        public void SearchUserGroupForAdd()
        {
            this.Get(this.ObjDetail.USER_NAME);
            var lstUserGroupOfUser = this.ObjDetail.ListUserUserGroup.Select(x => x.USER_GROUP_CODE).ToList();
            var query = UnitOfWork.Repository<UserGroupRepo>().Queryable();
            query = query.Where(x => !lstUserGroupOfUser.Contains(x.CODE));
            if (!string.IsNullOrWhiteSpace(ObjUserGroup.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(ObjUserGroup.CODE.ToLower()) || x.NAME.ToLower().Contains(ObjUserGroup.CODE.ToLower()));
            }
            this.ObjListUserGroup = query.ToList();
        }

        public void AddUserGroupToUser(string lstUserGroup, string userName)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var userGroupCode in lstUserGroup.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
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
                this.State = false;
                this.Exception = ex;
            }
        }

        public override void Create()
        {
            try
            {
                if (!this.CheckExist(x => x.USER_NAME == this.ObjDetail.USER_NAME))
                {
                    this.ObjDetail.PASSWORD = UtilsCore.EncryptStringMD5(this.ObjDetail.USER_NAME + "@123");
                    this.ObjDetail.ACTIVE = true;
                    this.ObjDetail.OTP_VERIFY = true;
                    if (string.IsNullOrWhiteSpace(this.ObjDetail.COMPANY_ID) && !string.IsNullOrWhiteSpace(this.COMPANY_ID))
                    {
                        this.ObjDetail.COMPANY_ID = this.COMPANY_ID;
                    }
                    base.Create();
                }
                else
                {
                    this.State = false;
                    this.MesseageCode = "1104";
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public override void Update()
        {
            try
            {
                var user = UnitOfWork.Repository<UserRepo>().Get(this.ObjDetail.USER_NAME);
                UnitOfWork.Repository<UserRepo>().Detach(user);

                UnitOfWork.BeginTransaction();
                if (string.IsNullOrWhiteSpace(this.ObjDetail.COMPANY_ID) && !string.IsNullOrWhiteSpace(this.COMPANY_ID))
                {
                    this.ObjDetail.COMPANY_ID = this.COMPANY_ID;
                }
                this.ObjDetail.PASSWORD = user.PASSWORD;
                this.ObjDetail = this.CurrentRepository.Update(this.ObjDetail);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void DeleteUserGroupOfUser(string lstUserGroup, string userName)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var userGroupCode in lstUserGroup.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
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

        public List<NodeUser> BuildTreeOrg()
        {
            var lstNode = new List<NodeUser>();
            var serviceOrganize = new OrganizeService();
            serviceOrganize.GetAll();
            foreach (var organize in serviceOrganize.ObjList.OrderBy(x => x.C_ORDER))
            {
                var nodeOrganize = new NodeUser()
                {
                    id = organize.PKID,
                    pId = organize.PARENT,
                    name = organize.NAME,
                    icon = "/Content/zTreeStyle/img/diy/donvi.gif",
                };
                lstNode.Add(nodeOrganize);
            }
            return lstNode;
        }

        public List<NodeRight> BuildTreeRight(string userName)
        {
            var lstNode = new List<NodeRight>();

            dynamic param = new ExpandoObject();
            param.IsFetch_ListUserUserGroup = true;
            param.IsFetch_ListUserRight = true;
            param.IsFetch_ListUserRole = true;

            this.Get(userName);

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
            //Danh sách role riêng của user
            foreach (var item in this.ObjDetail.ListUserRole)
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

            if (this.ObjDetail.IS_MODIFY_RIGHT)
            {
                var lstRightModify = this.ObjDetail.ListUserRight;
                foreach (var item in lstRightModify)
                {
                    if (item.IS_ADD && lstRoleDetail.Count(x => x.FK_RIGHT == item.FK_RIGHT) == 0)
                    {
                        var node = lstNode.Where(x => x.id == item.FK_RIGHT).FirstOrDefault();
                        node.@checked = "true";
                        node.isAdd = "1";
                    }

                    if (item.IS_REMOVE && lstRoleDetail.Count(x => x.FK_RIGHT == item.FK_RIGHT) >= 0)
                    {
                        var node = lstNode.Where(x => x.id == item.FK_RIGHT).FirstOrDefault();
                        node.@checked = "false";
                        node.isRemove = "1";
                    }
                }
            }

            return lstNode;
        }

        public void UpdateRightOfUser(string userName, string rightList, string statusList)
        {
            try
            {
                this.Get(userName);

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

                var lstStatus = statusList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var lstRight = rightList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                var lstRightModify = this.ObjDetail.ListUserRight;

                UnitOfWork.BeginTransaction();

                for (int i = 0; i < lstRight.Length; i++)
                {
                    var right = lstRight[i];
                    if (lstRightModify.Count(x => x.FK_RIGHT == right) > 0)
                    {
                        var righModify = lstRightModify.FirstOrDefault(x => x.FK_RIGHT == right);
                        lstRightModify.Remove(righModify);
                        UnitOfWork.Repository<UserRightRepo>().Delete(righModify);
                    }

                    if (lstStatus[i] == "true")
                    {
                        if (lstRoleDetail.Count(x => x.FK_RIGHT == right) == 0 && lstRightModify.Count(x => x.FK_RIGHT == right) == 0)
                        {
                            UnitOfWork.Repository<UserRightRepo>().Create(
                                new T_AD_USER_RIGHT()
                                {
                                    FK_RIGHT = right,
                                    USER_NAME = userName,
                                    IS_ADD = true,
                                    IS_REMOVE = false
                                }
                            );
                        }
                    }
                    else if (lstStatus[i] == "false")
                    {
                        if (lstRoleDetail.Count(x => x.FK_RIGHT == right) >= 0 && lstRightModify.Count(x => x.FK_RIGHT == right) == 0)
                        {
                            UnitOfWork.Repository<UserRightRepo>().Create(
                                new T_AD_USER_RIGHT()
                                {
                                    FK_RIGHT = right,
                                    USER_NAME = userName,
                                    IS_ADD = false,
                                    IS_REMOVE = true
                                }
                            );
                        }
                    }
                }

                this.ObjDetail.IS_MODIFY_RIGHT = true;
                UnitOfWork.Repository<UserRepo>().Update(this.ObjDetail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void ResetRightOfUser(string userName)
        {
            try
            {
                this.Get(userName);

                UnitOfWork.BeginTransaction();

                this.ObjDetail.IS_MODIFY_RIGHT = false;
                UnitOfWork.Repository<UserRepo>().Update(this.ObjDetail);

                foreach (var item in this.ObjDetail.ListUserRight)
                {
                    UnitOfWork.Repository<UserRightRepo>().Delete(item);
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

        public void ResetPassword(string userName)
        {
            try
            {
                this.Get(userName);
                UnitOfWork.BeginTransaction();
                this.ObjDetail.PASSWORD = UtilsCore.EncryptStringMD5(this.ObjDetail.USER_NAME + "@123");
                this.ObjDetail.LAST_CHANGE_PASS_DATE = DateTime.Now;
                UnitOfWork.Repository<UserRepo>().Update(this.ObjDetail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void SearchRoleForAdd()
        {
            this.Get(this.ObjDetail.USER_NAME);
            var lstRoleOfUser = this.ObjDetail.ListUserRole.Select(x => x.ROLE_CODE).ToList();
            foreach (var item in this.ObjDetail.ListUserUserGroup)
            {
                lstRoleOfUser.AddRange(item.UserGroup.ListUserGroupRole.Select(x => x.ROLE_CODE));
            }
            lstRoleOfUser = lstRoleOfUser.Distinct().ToList();

            var query = UnitOfWork.Repository<RoleRepo>().Queryable();
            query = query.Where(x => !lstRoleOfUser.Contains(x.CODE));
            if (!string.IsNullOrWhiteSpace(ObjRole.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(ObjRole.CODE.ToLower()) || x.NAME.ToLower().Contains(ObjRole.CODE.ToLower()));
            }
            this.ObjListRole = query.ToList();
        }

        public void SearchVendorForAdd()
        {
            //int startIndex = this.NumerRecordPerPage * (this.Page - 1);
            //dynamic param = new ExpandoObject();
            //param.IsFetch_ListUserVender = true;
            //this.Get(this.ObjDetail.USER_NAME, param);
            //var lstVendorOfUser = this.ObjDetail.ListUserVendor.Select(x => x.VENDOR_CODE).ToList();

            //var query = UnitOfWork.Repository<VendorOldRepo>().Queryable();
            //query = query.Where(x => !lstVendorOfUser.Contains(x.CODE));
            //if (!string.IsNullOrWhiteSpace(ObjVendor.CODE))
            //{
            //    query = query.Where(x => x.CODE.ToLower().Contains(ObjVendor.CODE.ToLower()) || x.TEXT.ToLower().Contains(ObjVendor.CODE.ToLower()));
            //}
            //query = query.OrderByDescending(x => x.ACTIVE);
            //var rowCount = query.ToFutureValue(x => x.Count());
            //this.ObjListVendor = query.Skip(startIndex).Take(this.NumerRecordPerPage).ToFuture().ToList<T_MD_VENDOR_OLD>();
            //this.TotalRecord = rowCount.Value;
        }

        public void SearchCustomerForAdd()
        {
            //int startIndex = this.NumerRecordPerPage * (this.Page - 1);

            //dynamic param = new ExpandoObject();
            //param.IsFetch_ListUserCustomer = true;
            //param.IsFetch_Organize = true;

            //this.Get(this.ObjDetail.USER_NAME, param);

            //if (this.ObjDetail.Organize == null || string.IsNullOrWhiteSpace(this.ObjDetail.Organize.PKID) )
            //{
            //    return;
            //}
            //var lstCustomerOfUser = this.ObjDetail.ListUserCustomer.Select(x => x.CUSTOMER_CODE).ToList();

            //var query = UnitOfWork.Repository<CustomerOldRepo>().Queryable();
            //query = query.Where(x => !lstCustomerOfUser.Contains(x.CUSTOMER_CODE));
            //if (!string.IsNullOrWhiteSpace(ObjCustomer.CUSTOMER_CODE))
            //{
            //    query = query.Where(x => x.CUSTOMER_CODE.ToLower().Contains(ObjCustomer.CUSTOMER_CODE.ToLower()) || x.TEXT.ToLower().Contains(ObjCustomer.CUSTOMER_CODE.ToLower()));
            //}

            //query = query.Where(x => x.COMPANY_CODE == this.ObjDetail.Organize.COMPANY_CODE);

            //query = query.OrderByDescending(x => x.ACTIVE);
            //var rowCount = query.ToFutureValue(x => x.Count());
            //this.ObjListCustomer = query.Skip(startIndex).Take(this.NumerRecordPerPage).ToFuture().ToList<T_MD_CUSTOMER_OLD>();
            //this.TotalRecord = rowCount.Value;

            //this.ObjListCustomer = query.OrderByDescending(x => x.ACTIVE).ToList();
        }

        public void AddCustomerToUser(string lstCustomer, string userName, string companyCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var customerCode in lstCustomer.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = new T_AD_USER_CUSTOMER()
                    {
                        COMPANY_CODE = companyCode,
                        CUSTOMER_CODE = customerCode,
                        USER_NAME = userName,
                        CREATE_BY = ProfileUtilities.User.USER_NAME
                    };

                    UnitOfWork.Repository<UserCustomerOldRepo>().Create(item);
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

        public void AddRoleToUser(string lstRole, string userName)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var roleCode in lstRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = new T_AD_USER_ROLE()
                    {
                        ROLE_CODE = roleCode,
                        USER_NAME = userName
                    };

                    if (ProfileUtilities.User != null)
                    {
                        item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        item.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                    }
                    UnitOfWork.Repository<UserRoleRepo>().Create(item);
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

        public void AddVendorToUser(string lstVendor, string userName)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var vendorCode in lstVendor.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = new T_AD_USER_VENDOR()
                    {
                        VENDOR_CODE = vendorCode,
                        USER_NAME = userName
                    };

                    if (ProfileUtilities.User != null)
                    {
                        item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                    }
                    UnitOfWork.Repository<UserVendorRepo>().Create(item);
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

        public void DeleteRoleOfUser(string lstRole, string userName)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var roleCode in lstRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = UnitOfWork.Repository<UserRoleRepo>().Queryable().FirstOrDefault(x => x.ROLE_CODE == roleCode && x.USER_NAME == userName);
                    if (item != null)
                    {
                        UnitOfWork.Repository<UserRoleRepo>().Delete(item);
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

        public void DeleteVendorOfUser(string lstRole, string userName)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var vendorCode in lstRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = UnitOfWork.Repository<UserVendorRepo>().Queryable().FirstOrDefault(x => x.VENDOR_CODE == vendorCode && x.USER_NAME == userName);
                    if (item != null)
                    {
                        UnitOfWork.Repository<UserVendorRepo>().Delete(item);
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

        public void DeleteCustomerOfUser(string lstRole, string userName)
        {
            try
            {
                dynamic param = new ExpandoObject();
                param.IsFetch_Organize = true;
                this.Get(userName, param);

                UnitOfWork.BeginTransaction();

                foreach (var customerCode in lstRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = UnitOfWork.Repository<UserCustomerOldRepo>().Queryable().FirstOrDefault(x => x.CUSTOMER_CODE == customerCode && x.USER_NAME == userName && x.COMPANY_CODE == this.ObjDetail.Organize.COMPANY_CODE);
                    if (item != null)
                    {
                        UnitOfWork.Repository<UserCustomerOldRepo>().Delete(item);
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

        public void UpdatePass()
        {
            try
            {
                this.ObjDetail.OLD_PASSWORD = UtilsCore.EncryptStringMD5(this.ObjDetail.OLD_PASSWORD);
                var user = UnitOfWork.Repository<UserRepo>().Queryable().FirstOrDefault(x => x.USER_NAME == this.ObjDetail.USER_NAME && x.PASSWORD == this.ObjDetail.OLD_PASSWORD);
                if (user == null)
                {
                    this.State = false;
                    this.MesseageCode = "1103";
                    return;
                }
                UnitOfWork.BeginTransaction();
                user.PASSWORD = UtilsCore.EncryptStringMD5(this.ObjDetail.PASSWORD);
                user.LAST_CHANGE_PASS_DATE = DateTime.Now;
                this.CurrentRepository.Update(user);
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
