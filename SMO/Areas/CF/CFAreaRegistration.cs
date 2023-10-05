using System.Web.Mvc;

namespace SMO.Areas.CF
{
    public class CFAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CF";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CF_default",
                "CF/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}