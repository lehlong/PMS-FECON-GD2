using System.Web.Mvc;
using SMO.Service.AD;
using System.Web.Script.Serialization;
using System.Dynamic;
using SMO.Core.Entities;
using System.Linq;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom(Right = "R1.4")]
    public class UserController : Controller
    {
        private UserService _service;

        public UserController()
        {
            _service = new UserService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexOrganize()
        {
            return PartialView(_service);
        }

        public ActionResult BuildTreeOrg()
        {
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            var lstNodeOrg = _service.BuildTreeOrg();
            ViewBag.zNodeOrg = oSerializer.Serialize(lstNodeOrg);
            return PartialView();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult BuildTreeRight(string userName)
        {
            var lstNode = _service.BuildTreeRight(userName);
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            oSerializer.MaxJsonLength = int.MaxValue;
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            return PartialView();
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(UserService service)
        {
            service.Search();
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListUserGroupOfUser(UserService service)
        {
            dynamic param = new ExpandoObject();
            param.IsFetch_ListUserUserGroup = true;
            service.Get(service.ObjDetail.USER_NAME, param);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListRoleOfUser(UserService service)
        {
            dynamic param = new ExpandoObject();
            param.IsFetch_ListUserUserGroup = true;
            param.IsFetch_ListUserRole = true;
            service.Get(service.ObjDetail.USER_NAME, param);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListVendorOfUser(UserService service)
        {
            dynamic param = new ExpandoObject();
            param.IsFetch_ListUserVendor = true;
            service.Get(service.ObjDetail.USER_NAME, param);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListCustomerOfUser(UserService service)
        {
            dynamic param = new ExpandoObject();
            param.IsFetch_ListUserCustomer = true;
            service.Get(service.ObjDetail.USER_NAME, param);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListRoleForAdd(UserService service)
        {
            service.SearchRoleForAdd();
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListVendorForAdd(UserService service)
        {
            service.SearchVendorForAdd();
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListCustomerForAdd(UserService service)
        {
            service.SearchCustomerForAdd();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddRole(string userName)
        {
            _service.ObjDetail.USER_NAME = userName;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddVendor(string userName)
        {
            _service.ObjDetail.USER_NAME = userName;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddCustomer(string userName, string companyCode)
        {
            dynamic param = new ExpandoObject();
            param.IsFetch_Organize = true;
            _service.Get(userName, param);
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddCustomerToUser(string lstCustomer, string userName, string companyCode)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.AddCustomerToUser(lstCustomer, userName, companyCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListCustomerForAdd(); SubmitListCustomerOfUser();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddRoleToUser(string lstRole, string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.AddRoleToUser(lstRole, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListRoleForAdd(); SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddVendorToUser(string lstVendor, string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.AddVendorToUser(lstVendor, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListVendorForAdd(); SubmitListVendorOfUser();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteRoleOfUser(string lstRole, string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteRoleOfUser(lstRole, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteVendorOfUser(string lstVendor, string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteVendorOfUser(lstVendor, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListVendorOfUser()";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteCustomerOfUser(string lstCustomer, string userName, string companyCode)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteCustomerOfUser(lstCustomer, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListCustomerOfUser();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListUserGroupForAdd(UserService service)
        {
            service.SearchUserGroupForAdd();
            return PartialView(service);
        }

        public ActionResult AddUserGroup(string userName)
        {
            _service.ObjDetail.USER_NAME = userName;
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddUserGroupToUser(string lstUserGroup, string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.AddUserGroupToUser(lstUserGroup, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListUserGroupForAdd(); SubmitListUserGroupOfUser(); SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create(string orgId)
        {
            var findOrg = _service.UnitOfWork.GetSession().Query<T_AD_ORGANIZE>().FirstOrDefault(x => x.PKID == orgId);
            if (findOrg != null)
            {
                if (findOrg.TYPE == "CP")
                {
                    _service.COMPANY_ID = findOrg.PKID;
                }
                else
                {
                    _service.ObjDetail.COMPANY_ID = findOrg.PKID;
                    var findOrgParent = _service.UnitOfWork.GetSession().Query<T_AD_ORGANIZE>().FirstOrDefault(x => x.COMPANY_CODE == findOrg.COMPANY_CODE && x.TYPE == "CP");
                    _service.COMPANY_ID = findOrgParent?.PKID;
                }
            }
            return PartialView(_service);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                dynamic param = new ExpandoObject();
                param.IsFetch_ListUserUserGroup = true;
                param.IsFetch_ListUserRight = true;
                param.IsFetch_ListUserRole = true;
                param.IsFetch_ListUserCustomer = true;
                _service.Get(id, param);

                var findOrg = _service.UnitOfWork.GetSession().Query<T_AD_ORGANIZE>().FirstOrDefault(x => x.PKID == _service.ObjDetail.COMPANY_ID);
                if (findOrg != null)
                {
                    if (findOrg.TYPE == "CP")
                    {
                        _service.COMPANY_ID = findOrg.PKID;
                    }
                    else
                    {
                        var findOrgParent = _service.UnitOfWork.GetSession().Query<T_AD_ORGANIZE>().FirstOrDefault(x => x.COMPANY_CODE == findOrg.COMPANY_CODE && x.TYPE == "CP");
                        _service.COMPANY_ID = findOrgParent?.PKID;
                    }
                }
            }
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateRightOfUser(string userName, string rightList, string statusList)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.UpdateRightOfUser(userName, rightList, statusList);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "BuildTreeRight(); $('#cmdResetQuyen').show();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult ResetRightOfUser(string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.ResetRightOfUser(userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "BuildTreeRight(); $('#cmdResetQuyen').hide();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult ResetPassword(string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.ResetPassword(userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UserService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string pStrListSelected)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.Delete(pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult DeleteUserGroupOfUser(string lstUserGroup, string userName)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteUserGroupOfUser(lstUserGroup, userName);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListUserGroupOfUser(); SubmitListRoleOfUser(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public virtual ActionResult ToggleActive(string id)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.ToggleActive(id);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
    }
}