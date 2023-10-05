using SMO.Service.PS;

using System;
using System.Web.Mvc;

namespace SMO.Areas.GC.Controllers
{
    public class GanttChartController : Controller
    {
        private readonly ProjectStructService _service;
        public GanttChartController()
        {
            _service = new ProjectStructService();
        }
        // GET: GC/GanttChart
        public ActionResult Index(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.CurrentUser = ProfileUtilities.User.USER_NAME;
            return PartialView();
        }
        public ActionResult IndexBoq(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.CurrentUser = ProfileUtilities.User.USER_NAME;
            return PartialView();
        }

        public ActionResult IndexView(Guid projectId)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.CurrentUser = ProfileUtilities.User.USER_NAME;
            return PartialView();
        }
    }

}