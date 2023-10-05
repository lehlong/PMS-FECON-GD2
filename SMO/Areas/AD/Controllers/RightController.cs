using SMO.Service.AD;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom(Right = "R1.2")]
    public class RightController : Controller
    {
        private RightService _service;

        public RightController()
        {
            _service = new RightService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult BuildTree(string rightSelected)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var lstNode = _service.BuildTree();
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            oSerializer.MaxJsonLength = int.MaxValue;
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            ViewBag.RightSelected = rightSelected;
            return PartialView();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create(string parent)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            _service.ObjDetail.PARENT = parent;
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RightService service)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = string.Format("BuildTree('{0}', true);", service.ObjDetail.CODE);
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
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(RightService service)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = string.Format("BuildTree('{0}', true);", service.ObjDetail.CODE);
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
        public ActionResult Delete(string code)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            result.State = true;
            _service.Delete(code);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = string.Format("BuildTree('', true);");
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
        public ActionResult UpdateTree(List<NodeRight> lstNode)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var result = new TransferObject();
            result.State = true;
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.UpdateTree(lstNode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "";
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