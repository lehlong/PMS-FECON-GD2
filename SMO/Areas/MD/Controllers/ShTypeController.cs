using System.Web.Mvc;
using SMO.Service.MD;

namespace SMO.Areas.MD.Controllers
{
    [Authorize]
    public class ShTypeController : Controller
    {
        private ShTypeService _service;

        public ShTypeController()
        {
            _service = new ShTypeService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(ShTypeService service)
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
            _service.Synchronize();
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