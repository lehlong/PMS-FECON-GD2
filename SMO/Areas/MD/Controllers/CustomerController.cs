using System;
using System.Web.Mvc;
using SMO.Service.MD;

namespace SMO.Areas.MD.Controllers
{

    public class CustomerController : Controller
    {
        private CustomerService _service;

        public CustomerController()
        {
            _service = new CustomerService();
        }
        [MyValidateAntiForgeryToken]
        public ActionResult IndexProjectVolume(Guid projectId, bool isCustomer, bool isAccept)
        {
            _service.ProjectId = projectId;
            _service.IsAccept = isAccept;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListProjectVolume(CustomerService service)
        {
            service.SearchProjectVolume();
            return PartialView(service);
        }
        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R2.11")]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R2.11")]
        public ActionResult List(CustomerService service)
        {
            service.Search();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R2.11")]
        public ActionResult Create()
        {
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeCustom(Right = "R2.11")]
        public ActionResult Create(CustomerService service)
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
        [AuthorizeCustom(Right = "R2.11")]
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
        [AuthorizeCustom(Right = "R2.11")]
        public ActionResult Update(CustomerService service)
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
        [AuthorizeCustom(Right = "R2.11")]
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
        [AuthorizeCustom(Right = "R2.11")]
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