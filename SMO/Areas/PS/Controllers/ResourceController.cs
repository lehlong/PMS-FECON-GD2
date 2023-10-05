using SMO.Service.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SMO.Areas.PS.Controllers
{
    public class ResourceController : Controller
    {
        private readonly ProjectResourceService _service;

        public ResourceController()
        {
            _service = new ProjectResourceService();
        }

        // Danh sách nhận sự fecon
        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid projectId, string modulName)
        {
            ViewBag.ModulName = modulName;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        // Danh sách nhận sự nhà thầu
        [MyValidateAntiForgeryToken]
        public ActionResult IndexFecon(Guid projectId)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        // Danh sách nhận sự nhà thầu
        [MyValidateAntiForgeryToken]
        public ActionResult IndexVendor(Guid projectId)
        {
            var serviceOther = new ProjectResourceOtherService();
            serviceOther.ObjDetail.PROJECT_ID = projectId;
            return PartialView(serviceOther);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexView(Guid projectId)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListFecon(ProjectResourceService service)
        {
            service.Search();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult UpdateFecon(ProjectResourceService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                //result.ExtData = $"SubmitIndexResource();$('#{service.ViewId} #IsSaveOther').val('False');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListVendor(ProjectResourceOtherService service)
        {
            service.Search();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult UpdateOther(ProjectResourceOtherService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = $"try{{SubmitIndexResourceVendor();$('#{service.ViewId} #IsSaveOther').val('False');}}catch{{}}";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                result.ExtData = $"$('#{service.ViewId} #IsSaveOther').val('False');";
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListView(ProjectResourceService service)
        {
            service.Search();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create()
        {
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectResourceService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.ObjDetail.ID = Guid.NewGuid();
            service.Create();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddResourceUser(Guid projectId, string userType)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.ObjUserFilter.USER_TYPE = userType;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListUser(ProjectResourceService service)
        {
            service.Search();
            service.SearchUser();
            return PartialView(service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddResources(Guid projectId, string pStrListSelected)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.AddResources(projectId, pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "try{SubmitIndex();SubmitIndexResource();SubmitIndexResourceVendor();}catch{}";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        public ActionResult EditRight(Guid projectId, string userName)
        {
            ViewBag.ProjectId = projectId;
            ViewBag.UserName = userName;
            var lstNode = _service.BuildTreeRight(projectId, userName);
            JavaScriptSerializer oSerializer = new JavaScriptSerializer();
            oSerializer.MaxJsonLength = int.MaxValue;
            ViewBag.zNode = oSerializer.Serialize(lstNode);
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateRight(string userName, Guid projectId, string rightList, string statusList)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.UpdateRight(userName, projectId, rightList, statusList);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }


        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string pStrListSelected)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.Delete(pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                result.ExtData = "try{SubmitIndex();SubmitIndexResource();SubmitIndexResourceVendor();}catch{}";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }
    }
}
