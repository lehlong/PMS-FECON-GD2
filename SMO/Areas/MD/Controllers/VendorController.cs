using System;
using System.Web.Mvc;
using SMO.Service.MD;

namespace SMO.Areas.MD.Controllers
{

    public class VendorController : Controller
    {
        private VendorService _service;

        public VendorController()
        {
            _service = new VendorService();
        }

        [AuthorizeCustom(Right = "R2.12")]
        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [AuthorizeCustom(Right = "R2.12")]
        [ValidateAntiForgeryToken]
        public ActionResult List(VendorService service)
        {
            service.Search();
            return PartialView(service);
        }
        [MyValidateAntiForgeryToken]
        public ActionResult IndexProjectVolume(Guid projectId, bool isAccept)
        {
            _service.ProjectId = projectId;
            _service.IsAccept = isAccept;
            _service.SearchProjectVolume();
            return PartialView(_service);
        }

        //[ValidateAntiForgeryToken]
        //public ActionResult ListProjectVolume(VendorService service)
        //{
        //    service.SearchProjectVolume();
        //    return PartialView(service);
        //}

        [AuthorizeCustom(Right = "R2.12")]
        [MyValidateAntiForgeryToken]
        public ActionResult Create()
        {
            return PartialView(_service);
        }

        [HttpPost]
        [AuthorizeCustom(Right = "R2.12")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VendorService service)
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
        [AuthorizeCustom(Right = "R2.12")]
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
        [AuthorizeCustom(Right = "R2.12")]
        public ActionResult Update(VendorService service)
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
        [AuthorizeCustom(Right = "R2.12")]
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
        [AuthorizeCustom(Right = "R2.12")]
        public virtual ActionResult ToggleActive(string id)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.ToggleActive(id);
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
    }
}