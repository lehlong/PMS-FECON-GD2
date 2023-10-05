using System.Web.Mvc;
using SMO.Service.MD;

namespace SMO.Areas.MD.Controllers
{
    [Authorize]
    public class ContractController : Controller
    {
        private ContractService _service;

        public ContractController()
        {
            _service = new ContractService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(ContractService service)
        {
            service.Search();
            return PartialView(service);
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
        public virtual ActionResult Synchronize()
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.Synchronize("6610");
            _service.Synchronize("6630");
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