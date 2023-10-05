using System.Web.Mvc;

namespace SMO.Areas.GC
{
    public class GCAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "GC";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "GC_default",
                "GC/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}