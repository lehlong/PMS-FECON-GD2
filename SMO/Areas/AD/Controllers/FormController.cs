using System.Web.Mvc;
using SMO.Service.AD;
using System;
using SMO;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom(Right = "R1.6")]
    public class FormController : Controller
    {
        private FormService _service;

        public FormController()
        {
            _service = new FormService();
        }

        //[AuthorizeCustom(Right = "TEST")]
        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(FormService service)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            service.Search();
            return PartialView(service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListObject(FormService service)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            service.SearchObject();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create()
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult CreateObject(string formCode)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            _service.ObjObject.FK_FORM = formCode;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Copy(string formCode)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            _service.Get(formCode);
            _service.ObjDetail.CODE = "";
            _service.ObjDetail.NAME += "_Copy";
            _service.IsCopy = true;
            _service.FormCopy = formCode;
            return PartialView("Create", _service);
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FormService service)
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
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateObject(FormService service)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.CreateObject();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = "SubmitListObject();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult EditObject(string id)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.GetObjectById(id);
            }
            return PartialView(_service);
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
                _service.ObjObject.FK_FORM = id;
            }
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateObject(FormService service)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.UpdateObject();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "SubmitListObject();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(FormService service)
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
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
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
        public ActionResult DeleteObject(string pStrListSelected)
        {
            if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            {
                return Content("Chức năng hệ thống không được phép truy cập!");
            }
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteObject(pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "SubmitListObject();";
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