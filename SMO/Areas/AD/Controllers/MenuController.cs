using SMO.Service.AD;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom(Right = "R1.8")]
    public class MenuController : Controller
    {
        private MenuService _service;

        public MenuController()
        {
            _service = new MenuService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult BuildTree(string menuSelected)
        {
            var lstNode = _service.BuildTree();
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            oSerializer.MaxJsonLength = int.MaxValue;
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            ViewBag.MenuSelected = menuSelected;
            return PartialView();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create(string parent)
        {
            _service.ObjDetail.PARENT = parent;
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MenuService service)
        {
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
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(MenuService service)
        {
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
        public ActionResult UpdateTree(List<NodeMenu> lstNode)
        {
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