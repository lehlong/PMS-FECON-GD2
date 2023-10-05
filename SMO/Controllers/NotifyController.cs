using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SMO.Hubs;
using SMO.Service.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Controllers
{
    [System.Web.Mvc.Authorize]
    public class NotifyController : Controller
    {
        private NotifyService _service;
        public NotifyController()
        {
            _service = new NotifyService();
        }

        //public ActionResult SendNotify(string lstUserName)
        //{
        //    SMOUtilities.SendNotify(lstUserName.Split(',').ToList());
        //    return Content("");
        //}

        public ActionResult Index() {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(NotifyService service)
        {
            service.ObjDetail.USER_NAME = ProfileUtilities.User.USER_NAME;
            service.Search();
            return PartialView(service);
        }
    }
}