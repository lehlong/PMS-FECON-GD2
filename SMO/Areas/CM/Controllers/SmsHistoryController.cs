using SMO.Service.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.CM.Controllers
{
    [AuthorizeCustom(Right = "R193")]
    public class SmsHistoryController : Controller
    {
        private SmsHistoryService _service;
        public SmsHistoryController()
        {
            _service = new SmsHistoryService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(SmsHistoryService service)
        {
            service.Search();
            return PartialView(service);
        }
    }
}