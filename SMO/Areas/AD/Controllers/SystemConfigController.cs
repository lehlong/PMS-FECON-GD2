using SMO.Service.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.AD.Controllers
{
    [AuthorizeCustom(Right = "R1.1")]
    public class SystemConfigController : Controller
    {
        private SystemConfigService _service;

        public SystemConfigController()
        {
            _service = new SystemConfigService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            //string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            //if ((ip != "127.0.0.1" & ip != "::1"))
            //{
            //    if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            //    {
            //        ViewBag.Error = "Chỉ được phép cấu hình hệ thống tại máy chủ!";
            //        return PartialView("Error");
            //    }
            //}
            _service.GetConfig();
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateConfig(SystemConfigService service)
        {
            //string ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            //if ((ip != "127.0.0.1" & ip != "::1"))
            //{
            //    if (ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
            //    {
            //        ViewBag.Error = "Chỉ được phép cấu hình hệ thống tại máy chủ!";
            //        return PartialView("Error");
            //    }
            //}
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