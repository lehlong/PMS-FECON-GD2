using SMO.App_Start;
using SMO.SAPINT;
using SMO.Service.AD;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SMO
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
            InitSystem();
            InitMessage();
            InitLanguage();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app != null && app.Context != null)
            {
                app.Context.Response.Headers.Remove("Server");
            }
        }

        protected void InitMessage()
        {
            var service = new MessageService();
            service.GetAll();
            foreach (var item in service.ObjList)
            {
                MessageUtilities.AddToCache(new MessageObject()
                {
                    Code = item.CODE,
                    Language = item.LANGUAGE,
                    Message = item.MESSAGE
                });
            }
        }

        protected void InitLanguage()
        {
            var service = new LanguageService();
            service.GetAll();
            foreach (var item in service.ObjList)
            {
                LanguageUtilities.AddToCache(new LanguageObject()
                {
                    Code = item.OBJECT_TYPE + "-" + item.FORM_CODE + "-" + item.FK_CODE,
                    Language = item.LANG,
                    Value = item.VALUE
                });
            }
        }

        protected void InitSystem()
        {
            AuthorizeUtilities.IGNORE_USERS = new List<string> { "admin"};

            const string CsFileExtensions = "cshtml";
            ViewEngines.Engines.Clear();
            var razorEngine = new RazorViewEngine() { FileExtensions = new[] { CsFileExtensions } };
            ViewEngines.Engines.Add(razorEngine);

            ModelBinders.Binders.Add(typeof(string), new TrimStringModelBinder());

            log4net.Config.XmlConfigurator.Configure();

            var service = new SystemConfigService();
            service.GetConfig();
            SAPDestitination.Init(service.ObjDetail.SAP_HOST, service.ObjDetail.SAP_CLIENT, service.ObjDetail.SAP_NUMBER);
        }
    }
}
