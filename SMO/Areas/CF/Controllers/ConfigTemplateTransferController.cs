using SMO.Service.AD;
using SMO.Service.CF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.CF.Controllers
{
    [AuthorizeCustom(Right = "R111")]
    public class ConfigTemplateTransferController : Controller
    {
        private ConfigTemplateTransferService _service;

        public ConfigTemplateTransferController()
        {
            _service = new ConfigTemplateTransferService();
        }

        public ActionResult Index()
        {
            _service.GetTemplate();
            return PartialView(_service);
        }

        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ConfigTemplateTransferService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }
    }
}