using System.Web.Mvc;

namespace SMO.Areas.PS
{
    public class PSAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "PS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "PS_default",
                "PS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}