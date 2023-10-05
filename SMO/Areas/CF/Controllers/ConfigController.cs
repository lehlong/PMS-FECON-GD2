using SMO.Service.CF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SMO.Areas.CF.Controllers
{
    public class ConfigController : Controller
    {
        private ConfigService _service;
        public ConfigController()
        {
            _service = new ConfigService();
        }

        public ActionResult Index()
        {
            return View(_service);
        }

        public ActionResult BuildTree(string selected)
        {
            var lstNode = _service.BuildTree();
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            oSerializer.MaxJsonLength = int.MaxValue;
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            ViewBag.Selected = selected;
            return PartialView();
        }

        public ActionResult Create(string companyCode, string modulType)
        {
            _service.CompanyCode = companyCode;
            _service.ModulType = modulType;
            _service.GetInfoOrganzie();
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ConfigService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                //result.ExtData = string.Format("BuildTree('{0}', true);", service.ObjDetail.PKID);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }
    }
}