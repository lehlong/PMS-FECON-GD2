using SMO.Service.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class ProjectTimeController : Controller
    {
        private readonly ProjectTimeService _service;
        public ProjectTimeController()
        {
            _service = new ProjectTimeService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid projectId)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(ContractService service)
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
        public ActionResult Create(ContractService service)
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
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }

            return PartialView(_service);
        }
    }
}
