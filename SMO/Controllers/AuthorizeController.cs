using Microsoft.Ajax.Utilities;
using SMO.Service;
using SMO.Service.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SMO.Controllers
{
    public class AuthorizeController : Controller
    {
        public ActionResult Login(string ReturnUrl)
        {
            AuthorizeService _service = new AuthorizeService();
            if (Request.IsAuthenticated)
            {
                return Redirect("/");
            }
            if (Request.Cookies["Login"] != null)
            {
                _service.ObjUser.USER_NAME = Request.Cookies["Login"].Values["USER_NAME"];
                //_models.User.PASSWORD = Request.Cookies["Login"].Values["PASSWORD"];
            }
            _service.ReturnUrl = ReturnUrl;
            return PartialView(_service);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        //[ValidateRecaptcha]
        public ActionResult Login(AuthorizeService service)
        {
            //if (ModelState.ContainsKey("Recaptcha"))
            //{
            //    ViewBag.Error = "3";
            //    return PartialView(service);
            //}
            var historyService = new UserHistoryService();
            historyService.ObjDetail.PKID = Guid.NewGuid().ToString();
            historyService.ObjDetail.USER_NAME = service.ObjUser.USER_NAME;
            historyService.ObjDetail.LOGON_TIME = DateTime.Now;
            historyService.ObjDetail.BROWSER = Request.Browser.Browser;
            historyService.ObjDetail.VERSION = Request.Browser.Version;
            historyService.ObjDetail.IS_MOBILE = Request.Browser.IsMobileDevice;
            historyService.ObjDetail.MOBILE_MODEL = Request.Browser.MobileDeviceModel;
            historyService.ObjDetail.MANUFACTURER = Request.Browser.MobileDeviceManufacturer;
            historyService.ObjDetail.OS = Request.Browser.Platform;
            historyService.ObjDetail.IP_ADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["X_FORWARDED_FOR"];
            //if (string.IsNullOrEmpty(historyService.ObjDetail.IP_ADDRESS))
            //{
            //    historyService.ObjDetail.IP_ADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["X_FORWARDED_FOR"];
            //}
            if (string.IsNullOrEmpty(historyService.ObjDetail.IP_ADDRESS))
            {
                historyService.ObjDetail.IP_ADDRESS = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            service.IsValid();
            if (service.State)
            {
                if (!service.ObjUser.ACTIVE)
                {
                    ViewBag.Error = "2";
                    historyService.ObjDetail.STATUS = false;
                    historyService.Create();
                    return PartialView(service);
                }
                
                //Context.User.Identity.
                FormsAuthentication.SetAuthCookie(service.ObjUser.USER_NAME, service.IsRemember);
                if (!service.ObjUser.IS_IGNORE_USER)
                {
                    service.GetUserRight();
                    service.GetUserProjectRight();
                }
                //_models.GetUserRight();

                if (Request["User.REMEMBER"] == "on")
                {
                    HttpCookie cookie = new HttpCookie("Login");
                    cookie.Values.Add("USER_NAME", service.ObjUser.USER_NAME);
                    //cookie.Values.Add("PASSWORD", _models.User.PASSWORD);
                    cookie.Expires = DateTime.Now.AddDays(15);
                    Response.Cookies.Add(cookie);
                }
                ProfileUtilities.User = service.ObjUser;
                ProfileUtilities.UserRight = service.ListUserRight;
                ProfileUtilities.UserProjectRight = service.ListUserProjectRight;
                service.ReturnUrl = string.IsNullOrWhiteSpace(service.ReturnUrl) ? "/" : service.ReturnUrl;
                historyService.ObjDetail.STATUS = true;
                historyService.Create();

                if (Global.ListIPLogin.ContainsKey(historyService.ObjDetail.IP_ADDRESS))
                {
                    Global.ListIPLogin[historyService.ObjDetail.IP_ADDRESS] = 0;
                }
                return Redirect(service.ReturnUrl);
            }
            else
            {
                ViewBag.Error = service.ErrorMessage;
                historyService.ObjDetail.STATUS = false;
                historyService.Create();
                if (Global.ListIPLogin.ContainsKey(historyService.ObjDetail.IP_ADDRESS))
                {
                    Global.ListIPLogin[historyService.ObjDetail.IP_ADDRESS] += 1;
                }
                else
                {
                    Global.ListIPLogin.Add(historyService.ObjDetail.IP_ADDRESS, 1);
                }
            }
            return PartialView(service);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            FormsAuthentication.SignOut();
            return Redirect(FormsAuthentication.LoginUrl);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            var service = new UserService();
            service.ObjDetail.USER_NAME = ProfileUtilities.User.USER_NAME;
            service.ObjDetail.FULL_NAME = ProfileUtilities.User.FULL_NAME;
            service.ObjDetail.ACCOUNT_AD = ProfileUtilities.User.ACCOUNT_AD;

            return PartialView(service);
        }

        /// <summary>
        /// Cập nhật thông tin mật khẩu
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(UserService service)
        {
            var result = new TransferObject();
            if (service.ObjDetail.USER_NAME != ProfileUtilities.User.USER_NAME || string.IsNullOrWhiteSpace(service.ObjDetail.PASSWORD))
            {
                return Content("");
            }

            // Kiểm tra modelstate

            if (!ModelState.IsValid)
            {
                result.Type = TransferType.AlertDanger;
                service.ErrorMessage = string.Join("<br/>", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                SMOUtilities.GetMessage("1005", service, result);
                return result.ToJsonResult();
            }

            if (service.ObjDetail.PASSWORD != service.ObjDetail.RETRY_PASSWORD)
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1102", service, result);
                return result.ToJsonResult();
            }

            if (service.ObjDetail.PASSWORD.Length < 10)
            {
                result.Type = TransferType.AlertDanger;
                service.ErrorDetail = "Mật khẩu phải dài hơn 10 kí tự!";
                SMOUtilities.GetMessage("1005", service, result);
                return result.ToJsonResult();
            }

            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.UpdatePass();
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