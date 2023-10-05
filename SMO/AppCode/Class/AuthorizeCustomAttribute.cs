using SMO.Core.Entities;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace SMO
{
    public class AuthorizeCustomAttribute : AuthorizeAttribute
    {
        public string Right { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");
            var result = httpContext.Request.IsAuthenticated;
            if (result)
            {
                result = AuthorizeUtilities.CheckUserRight(this.Right);
            }
            return result;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    var result = new TransferObject
                    {
                        Type = TransferType.JsFunction,
                        ExtData = "AlertAndRedirectToLogin"
                    };
                    filterContext.Result = result.ToJsonResult();
                }
                else
                {
                    base.HandleUnauthorizedRequest(filterContext);
                    //if (!string.IsNullOrEmpty(session.UserName))
                    //{
                    //    filterContext.Controller.TempData["Message"] =
                    //        "Tài khoản này đã đăng nhập tại máy hoặc trình duyệt khác. Hãy kiểm tra lại tài khoản của bạn.";
                    //}
                }
                //FormsAuthentication.SignOut();
            }
            else
            {
                var profile = HttpContext.Current.Session["Profile"] as T_AD_USER;
                if (profile == null)
                {
                    base.HandleUnauthorizedRequest(filterContext);
                    if (filterContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var result = new TransferObject
                        {
                            Type = TransferType.JsFunction,
                            ExtData = "AlertAndRedirectToLogin"
                        };
                        filterContext.Result = result.ToJsonResult();
                    }
                    else
                    {
                        FormsAuthentication.SignOut();
                    }
                }
                else
                    base.OnAuthorization(filterContext);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                       { "action", "UnAuthorize" },
                                       { "controller", "Home" },
                                       { "area", ""},
                                       { "auth", Right},
                                   });
        }
    }
}