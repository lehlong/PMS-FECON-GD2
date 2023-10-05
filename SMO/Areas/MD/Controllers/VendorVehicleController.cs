using SMO.Service.AD;
using SMO.Service.MD;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.MD.Controllers
{
    [AuthorizeCustom(Right = "R1030")]
    public class VendorVehicleController : Controller
    {
        private VendorVehicleService _service;

        public VendorVehicleController()
        {
            _service = new VendorVehicleService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index()
        {
            return View();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult VTManager()
        {
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult CheckVehicle(VendorVehicleService service)
        {
            service.CheckVehicle();
            return PartialView(service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListVehicleVT(VendorVehicleService service)
        {
            service.GetListVehicleVT();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddToListVT(string vehicleCode, string vendorCode)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.AddToListVT(vehicleCode, vendorCode);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
                result.ExtData = "SubmitFormList();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }
    }
}