using SMO.Service.PS;
using SMO.Service.PS.Models;

using System;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class PlanProgressController : Controller
    {
        private readonly PlanProgressService _service;

        public PlanProgressController()
        {
            _service = new PlanProgressService();
        }
        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexView(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexPlanProgress(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexPlanProgressView(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;

            return PartialView(_service);
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public ActionResult EditPlanProgressVendor(PlanProgressService service)
        {
            var data = service.GetPlanProgresses();

            return PartialView(data);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult EditPlanProgressCustomer(PlanProgressService service)
        {
            var data = service.GetPlanProgresses();

            return PartialView(data);
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public ActionResult PlanProgressVendorView(PlanProgressService service)
        {
            var data = service.GetPlanProgresses();

            return PartialView(data);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult PlanProgressCustomerView(PlanProgressService service)
        {
            var data = service.GetPlanProgresses();

            return PartialView(data);
        }
    }
}
