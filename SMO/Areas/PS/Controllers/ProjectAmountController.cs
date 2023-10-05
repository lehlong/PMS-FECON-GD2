using SMO.Service.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class ProjectAmountController : Controller
    {
        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid projectId, bool isCustomer)
        {
            var planCostService = new PlanCostService();

            ViewBag.IsCustomer = isCustomer;
            planCostService.ObjDetail.PROJECT_ID = projectId;
            return PartialView(planCostService);
        }
        [MyValidateAntiForgeryToken]
        public ActionResult EditProjectAmountCustomer(Guid projectId)
        {
            var planCostService = new PlanCostService();
            planCostService.ObjDetail.PROJECT_ID = projectId;
            planCostService.ObjDetail.IS_CUSTOMER = true;

            //ViewBag.ProjectStructs = planCostService.GetProjectStruct();
            ViewBag.ProjectTimes = planCostService.GetProjectTime();
            return PartialView(planCostService);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult EditProjectAmountVendor(Guid projectId)
        {
            var planCostService = new PlanCostService();
            planCostService.ObjDetail.PROJECT_ID = projectId;
            planCostService.ObjDetail.IS_CUSTOMER = false;

            //ViewBag.ProjectStructs = planCostService.GetProjectStruct();
            ViewBag.ProjectTimes = planCostService.GetProjectTime();
            return PartialView(planCostService);
        }
    }
}
