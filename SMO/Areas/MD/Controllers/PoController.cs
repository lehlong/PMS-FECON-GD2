using System.Web.Mvc;
using SMO.Service.MD;

namespace SMO.Areas.MD.Controllers
{
    [Authorize]
    public class PoController : Controller
    {
        private PoService _service;

        public PoController()
        {
            _service = new PoService();
        }

        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(PoService service)
        {
            service.Search();
            return PartialView(service);
        }


        [HttpPost]
        [MyValidateAntiForgeryToken]
        public virtual ActionResult Synchronize()
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.Synchronize("6610", "P234");
            _service.Synchronize("6610", "P235");
            _service.Synchronize("6610", "P236");
            _service.Synchronize("6610", "P115");
            _service.Synchronize("6610", "P117");
            _service.Synchronize("6610", "P260");
            _service.Synchronize("6610", "P251");


            _service.Synchronize("6630", "P274");
            _service.Synchronize("6630", "P275");
            _service.Synchronize("6630", "P276");
            _service.Synchronize("6630", "P115");
            _service.Synchronize("6630", "P117");
            _service.Synchronize("6630", "P260");
            _service.Synchronize("6630", "P251");
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