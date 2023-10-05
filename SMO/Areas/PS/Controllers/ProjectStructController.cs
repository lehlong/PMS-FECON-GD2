using Newtonsoft.Json;
using SMO.Core.Entities.PS;
using SMO.Service.PS;
using SMO.Service.PS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class ProjectStructController : Controller
    {
        private readonly ProjectStructService _service;
        private readonly ProjectService _projectService;

        public ProjectStructController()
        {
            _service = new ProjectStructService();
            _projectService = new ProjectService();
        }

        public ActionResult Edit(Guid id)
        {
            _service.Get(id);
            if (_service.State)
            {
                ProjectEnum type;
                var canConvert = Enum.TryParse(_service.ObjDetail.TYPE, out type);
                if (canConvert)
                {
                    switch (type)
                    {
                        case ProjectEnum.PROJECT:
                            return PartialView("EditProject", _service);
                        case ProjectEnum.WBS:
                            return PartialView("EditWbs", _service);
                        case ProjectEnum.BOQ:
                            ViewBag.TotalPlanVolume = _service.CalculateTotalPlanVolume();
                            ViewBag.TotalWorkVolume = _service.CalculateTotalWorkVolume();
                            return PartialView("EditBoq", _service);
                        case ProjectEnum.ACTIVITY:
                            ViewBag.TotalPlanVolume = _service.CalculateTotalPlanVolume();
                            ViewBag.TotalWorkVolume = _service.CalculateTotalWorkVolume();
                            return PartialView("EditActivity", _service);
                        case ProjectEnum.CHECKLIST:
                        default:
                            break;
                    }
                }
            }
            return new EmptyResult();
        }

        public ActionResult ImportMsProject(Guid projectId, ProjectStructureType type)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.Type = type;
            return PartialView(_service);
        }

        [HttpPost]
        public ActionResult ImportFile(ProjectStructService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            if (Request.Files.Count == 0)
            {
                service.State = false;
                service.ErrorMessage = "Hãy chọn file excel!";
            }
            else if (Request.Files.Count > 1)
            {
                service.State = false;
                service.ErrorMessage = "Chỉ được phép chọn 1 file excel!";
            }
            else
            {
                service.ImportMsProject(Request);
            }

            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                var eventExec = service.Type == ProjectStructureType.COST ? "onClickTree()" : "onClickTreeBoq()";
                result.ExtData = $"{eventExec};$('.modal').modal('hide');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveActivity(ProjectStructService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.SaveActivity();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = $"refreshData('{service.ObjDetail.ID}');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBoq(ProjectStructService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.SaveBoq();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = $"refreshData('{service.ObjDetail.ID}');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveWbs(ProjectStructService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.SaveWbs();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = $"refreshData('{service.ObjDetail.ID}');";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveProject(ProjectStructService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.SaveProject();
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = "SubmitIndex();refreshData();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateStatuses(Guid projectId, string status, IList<Guid> structuresId)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateStatuses(projectId, status, structuresId);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        public ActionResult SaveActivityFromContractVendor(string data)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            List<AddNewTaskModel> lstTaskNew = (List<AddNewTaskModel>)Newtonsoft.Json.JsonConvert.DeserializeObject(data, typeof(List<AddNewTaskModel>));

            if(lstTaskNew.FirstOrDefault().ProjectId != null)
            {
                _service.SaveActivityFromContractVendor(lstTaskNew);
                _projectService.PostDataToSAP(lstTaskNew.FirstOrDefault().ProjectId);
            }
            
            if (_service.State && _projectService.State)
            {
                SMOUtilities.GetMessage("1001", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", _service, result);
                SMOUtilities.GetMessage("1004", _projectService, result);
            }
            return result.ToJsonResult();
        }
    }
}
