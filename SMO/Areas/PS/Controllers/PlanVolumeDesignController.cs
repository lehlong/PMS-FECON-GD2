using SMO.Service.PS;
using SMO.Service.PS.Models;

using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class PlanVolumeDesignController : Controller
    {
        private readonly PlanVolumeDesignService _service;

        public PlanVolumeDesignController()
        {
            _service = new PlanVolumeDesignService();
        }
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateValue(UpdatePlanDesignValueModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateValue(model);
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
