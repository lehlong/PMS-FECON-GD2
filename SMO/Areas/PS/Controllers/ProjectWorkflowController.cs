using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SMO.Core.Entities.PS;
using SMO.Service.PS;

namespace SMO.Areas.PS.Controllers
{
    //[AuthorizeCustom(Right = "R2.8")]

    public class ProjectWorkflowController : Controller
    {
        private ProjectWorkflowService _service;

        public ProjectWorkflowController()
        {
            _service = new ProjectWorkflowService();
        }

        [HttpGet]
        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid projectId, string modulName)
        {
            ViewBag.ModulName = modulName;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(ProjectWorkflowService service)
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
        public ActionResult Create(ProjectWorkflowService service)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
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

        [HttpPost]
        public ActionResult GenerateProjectWorkflow(Guid projectId)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.GenerateProjectWorkflow(projectId);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                _service.Get(Guid.Parse(id));
            }
            return PartialView(_service);
        }

        [HttpPost]
        public ActionResult Update(IList<T_PS_PROJECT_WORKFLOW_STEP> workflowStep)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.UpdateStep(workflowStep);
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
                result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public virtual ActionResult ToggleActive(string id)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.ToggleActive(Guid.Parse(id));
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
    }
}