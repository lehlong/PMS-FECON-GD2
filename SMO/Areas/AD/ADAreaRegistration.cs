using System.Web.Mvc;

namespace SMO.Areas.AD
{
    public class ADAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AD";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AD_default",
                "AD/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}