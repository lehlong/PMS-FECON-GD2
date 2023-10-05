using SMO.Service.PS;
using SMO.Service.PS.Models;

using System;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class PlanQuantityController : Controller
    {
        private readonly PlanQuantityService _service;

        public PlanQuantityController()
        {
            _service = new PlanQuantityService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexPlanQuantity(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexPlanQuantityView(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;

            return PartialView(_service);
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public ActionResult EditPlanQuantityVendor(PlanQuantityService service)
        {
            var data = service.GetPlanQuantities();
            ViewBag.ProjectTimes = service.GetProjectTime();
            ViewBag.ProjectId = service.ObjDetail.PROJECT_ID;
            return PartialView(data);
        }
        //[MyValidateAntiForgeryToken]
        public ActionResult EditPlanQuantityCustomer(PlanQuantityService service)
        {
            var data = service.GetPlanQuantities();
            ViewBag.ProjectTimes = service.GetProjectTime();
            ViewBag.ProjectId = service.ObjDetail.PROJECT_ID;

            return PartialView(data);
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public ActionResult PlanQuantityVendorView(PlanQuantityService service)
        {
            var data = service.GetPlanQuantities();
            ViewBag.ProjectTimes = service.GetProjectTime();
            ViewBag.ProjectId = service.ObjDetail.PROJECT_ID;

            return PartialView(data);
        }
        //[MyValidateAntiForgeryToken]
        public ActionResult PlanQuantityCustomerView(PlanQuantityService service)
        {
            var data = service.GetPlanQuantities();
            ViewBag.ProjectTimes = service.GetProjectTime();
            ViewBag.ProjectId = service.ObjDetail.PROJECT_ID;

            return PartialView(data);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateValue(UpdatePlanQuantityValueModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateValue(model);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
    }
}
