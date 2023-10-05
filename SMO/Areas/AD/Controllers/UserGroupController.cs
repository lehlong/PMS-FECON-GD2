using System.Web.Mvc;
using SMO.Service.AD;
using System.Web.Script.Serialization;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom (Right = "R1.5")]
    public class UserGroupController : Controller
    {
        private UserGroupService _service;

        public UserGroupController()
        {
            _service = new UserGroupService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult BuildTreeRight(string userGroupCode)
        {
            var lstNode = _service.BuildTreeRight(userGroupCode);
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            oSerializer.MaxJsonLength = int.MaxValue;
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult List(UserGroupService service)
        {
            service.Search();
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListUserOfGroup(UserGroupService service)
        {
            service.Get(service.ObjDetail.CODE);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListRoleOfUserGroup(UserGroupService service)
        {
            service.Get(service.ObjDetail.CODE);
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListUserForAdd(UserGroupService service)
        {
            service.SearchUserForAdd();
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ListRoleForAdd(UserGroupService service)
        {
            service.SearchRoleForAdd();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddUser(string userGroupCode)
        {
            _service.ObjDetail.CODE = userGroupCode;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddRole(string userGroupCode)
        {
            _service.ObjDetail.CODE = userGroupCode;
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddUserToGroup(string lstUser, string userGroupCode)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.AddUserToGroup(lstUser, userGroupCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListUserForAdd(); SubmitListUserOfGroup();";
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
        public ActionResult AddRoleToUserGroup(string lstRole, string userGroupCode)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.AddRoleToUserGroup(lstRole, userGroupCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitListRoleForAdd(); SubmitListRoleOfUserGroup(); BuildTreeRight();";
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
        public ActionResult DeleteUserOfGroup(string lstUser, string userGroupCode)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteUserOfGroup(lstUser, userGroupCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListUserOfGroup();";
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
        public ActionResult DeleteRoleOfUserGroup(string lstRole, string userGroupCode)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteRoleOfUserGroup(lstRole, userGroupCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListRoleOfUserGroup(); BuildTreeRight();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create()
        {
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserGroupService service)
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
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(UserGroupService service)
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
    }
}