using SMO.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO
{
    public class ValidateRecaptchaAttribute : ActionFilterAttribute
    {
        private const string RECAPTCHA_RESPONSE_KEY = "g-recaptcha-response";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            if (Global.ListIPLogin.ContainsKey(ip))
            {
                int count = 0;
                Global.ListIPLogin.TryGetValue(ip, out count);
                if (count < 6)
                {
                    return;
                }
                var isValidate = new RecaptchaValidationService(ConfigurationManager.AppSettings["RecaptchaSecretKey"]).Validate(filterContext.HttpContext.Request[RECAPTCHA_RESPONSE_KEY], ip);
                if (!isValidate)
                    filterContext.Controller.ViewData.ModelState.AddModelError("Recaptcha", "Captcha validation failed.");
            }
        }
    }
}