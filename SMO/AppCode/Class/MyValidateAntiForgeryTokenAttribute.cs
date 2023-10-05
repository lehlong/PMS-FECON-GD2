using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SMO
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MyValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter
    {


        private void ValidateRequestHeader(HttpRequestBase request)
        {
            string cookieToken = String.Empty;
            string formToken = String.Empty;
            string tokenValue = request.Headers["RequestVerificationToken"];
            if (!String.IsNullOrEmpty(tokenValue))
            {
                string[] tokens = tokenValue.Split(':');
                if (tokens.Length == 2)
                {
                    cookieToken = tokens[0].Trim();
                    formToken = tokens[1].Trim();
                }
            }
            AntiForgery.Validate(cookieToken, formToken);
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {

            try
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    ValidateRequestHeader(filterContext.HttpContext.Request);
                }
                else
                {
                    AntiForgery.Validate();
                }
            }
            catch
            {
                filterContext.Result = new ContentResult() { Content = "Phiên làm việc của bạn đã hết, vui lòng đăng nhập lại để tiếp tục sử dụng!" };
            }
        }
    }
}