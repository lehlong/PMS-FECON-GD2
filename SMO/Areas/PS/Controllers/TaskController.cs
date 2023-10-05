using SMO.Service.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class TaskController : Controller
    {
        private readonly TaskService _service;

        public TaskController()
        {
            _service = new TaskService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid activityParentId, Guid projectId)
        {
            _service.ObjDetail.ACTIVITY_PARENT_ID = activityParentId;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(TaskService service)
        {
            service.Search();
            return PartialView(service);
        }
        [MyValidateAntiForgeryToken]
        public ActionResult Create(Guid activityParentId, Guid projectId)
        {
            _service.ObjDetail.ACTIVITY_PARENT_ID = activityParentId;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TaskService service)
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
        public ActionResult Edit(Guid id)
        {
            _service.Get(id);
            return PartialView(_service);
        }
        [MyValidateAntiForgeryToken]
        public ActionResult GetDetail(Guid id)
        {
            _service.GetDetail(id);
            return new JsonNetResult()
            {
                Data = _service.ObjDetail,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(TaskService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = $"RefreshTask('{service.ObjDetail.ID}');";
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
