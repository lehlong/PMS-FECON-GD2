using SMO.Service.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private CommentService _service;

        public CommentController()
        {
            _service = new CommentService();
        }

        public ActionResult Index(string referenceId, string modulType, string poCode)
        {
            _service.ObjDetail.REFRENCE_ID = referenceId;
            _service.ObjDetail.MODUL_TYPE = modulType;
            _service.ObjDetail.PO_CODE = poCode;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(CommentService service)
        {
            service.GetComments();
            return PartialView(service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommentService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = string.Format("Forms.SubmitForm('{0}'); $('#txtContent').val('')", service.ObjDetail.REFRENCE_ID);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }
    }
}