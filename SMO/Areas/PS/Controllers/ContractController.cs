using SMO.Core.Entities.PS;
using SMO.Service.PS;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace SMO.Areas.PS.Controllers
{
    public class ContractController : Controller
    {
        private readonly ContractService _service;
        private readonly ProjectService _projectService;

        public ContractController()
        {
            _service = new ContractService();
            _projectService = new ProjectService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid projectId)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexCustomer(Guid projectId, string modulName)
        {
            ViewBag.ModulName = modulName;
            ViewBag.CheckContract = _service.CurrentRepository.Queryable().FirstOrDefault(x => x.PROJECT_ID == projectId && x.CONTRACT_TYPE.Contains("KD"));
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.ObjDetail.IS_SIGN_WITH_CUSTOMER = true;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexVendor(Guid projectId, string modulName)
        {
            ViewBag.ModulName = modulName;
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.ObjDetail.IS_SIGN_WITH_CUSTOMER = false;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListCustomer(ContractService service, string modulName)
        {
            ViewBag.ModulName = modulName;
            service.Search();
            return PartialView(service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListVendor(ContractService service, string modulName)
        {
            ViewBag.ModulName = modulName;
            service.Search();
            service.InitTotalValueBeforeTax();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexView(Guid projectId)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListView(ContractService service)
        {
            service.Search();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create(Guid projectId, string isCustomer, Guid? parentCode)
        {
            if (parentCode != null)
            {
                _service.Get(parentCode);
            }
            var project = _service.UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == projectId);
            _service.ObjDetail.PARENT_CODE = parentCode;
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.ObjDetail.CUSTOMER_CODE = project.CUSTOMER_CODE;
            _service.ObjDetail.IS_SIGN_WITH_CUSTOMER = false;
            _service.ObjDetail.CONTRACT_VALUE = 0;
            _service.ObjDetail.VAT = 0;
            _service.ObjDetail.CONTRACT_VALUE_VAT = 0;

            if (isCustomer == "1")
            {
                _service.ObjDetail.START_DATE = null;
                _service.ObjDetail.FINISH_DATE = null;
                _service.ObjDetail.CONTRACT_VALUE = 0;
                _service.ObjDetail.VAT= 0;
                _service.ObjDetail.CONTRACT_VALUE_VAT = 0;
                _service.ObjDetail.IS_SIGN_WITH_CUSTOMER = true;
            }
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Create(ContractService service, List<string> lstLink)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.ObjDetail.ID = Guid.NewGuid();
            service.ObjDetail.EXTEND_DATE = service.ObjDetail.FINISH_DATE;
            service.ObjDetail.REFERENCE_FILE_ID = Guid.NewGuid();
            service.Create(Request, lstLink ?? new List<string>());
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                if (service.ObjDetail.IS_SIGN_WITH_CUSTOMER)
                {
                    result.ExtData = $"onClickContractCus_Edit();SubmitIndexContractCustomer();LoadDetail('{service.ObjDetail.ID}')";
                }
                else
                {
                    result.ExtData = $"SubmitIndexContractVendor();LoadDetail('{service.ObjDetail.ID}')";
                }
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
        public ActionResult Update(ContractService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            if (service.ObjDetail.FINISH_DATE < service.ObjDetail.START_DATE)
            {
                service.State = false;
                service.ErrorMessage = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc";
            }
            else
            {
                service.Update();
            }
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                if (service.ObjDetail.IS_SIGN_WITH_CUSTOMER)
                {
                    result.ExtData = "SubmitIndexContractCustomer();";
                }
                else
                {
                    result.ExtData = "SubmitIndexContractVendor();";
                }
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, string modulName)
        {
            ViewBag.ModulName = modulName;
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult EditGeneralInformation(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
                ViewBag.CanEditContractCode = _service.CheckCanEditContractCode(id);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Detail(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            ViewBag.TreeContractProjectStructs = _service.BuildTreeContractProjectStructs();
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddTask(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult AddNewTask(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [ValidateAntiForgeryToken]
        public ActionResult ListTaskForAdd(ContractService service)
        {
            service.Get(service.ObjDetail.ID);
            service.SearchListTaskForAdd();
            return PartialView(service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddTaskToContract(Guid contractId, string pStrListSelected)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.AddTaskToContract(contractId, pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "SubmitIndexAddTask();onClickListTask();";
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
        public ActionResult RemoveTasksFromContract(Guid contractId, IList<Guid> structureIds)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.RemoveTasksFromContract(contractId, structureIds);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "onClickListTask();";
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
        public ActionResult RemoveTasksFromContractAndTree(Guid contractId, Guid projectId, IList<Guid> structureIds)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.RemoveTasksFromContractAndTree(contractId, structureIds);
            _projectService.PostDataToSAP(projectId);
            if (_service.State && _projectService.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = "onClickListTask();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _projectService, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult TreeContract(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            ViewBag.TreeContractProjectStructs = _service.BuildTreeContractProjectStructs();
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ListTask(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            ViewBag.TreeContractProjectStructs = _service.BuildTreeContractProjectStructs();
            return PartialView(_service);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateInformation(string data)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            List<UpdateContractModel> dataSave = (List<UpdateContractModel>)Newtonsoft.Json.JsonConvert.DeserializeObject(data, typeof(List<UpdateContractModel>));

            if(dataSave.FirstOrDefault() != null)
            {
                _service.UpdateContract(dataSave);
                _projectService.PostDataToSAP(dataSave.FirstOrDefault().ProjectId);
            }
            
            if (_service.State && _projectService.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                //result.ExtData = "onClickJob();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _projectService, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Delete(string pStrListSelected, string isCustomer)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            _service.DeleteContract(pStrListSelected);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1003", _service, result);
                if (isCustomer == "1")
                {
                    result.ExtData = $"onClickContractCus_Edit();";
                }
                else
                {
                    result.ExtData = $"onClickContractVen_Edit();";
                }
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1006", _service, result);
            }
            return result.ToJsonResult();
        }
        public ActionResult DownloadTemplateVendor()
        {
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template_Import_Activity_HopDongThauPhu.xlsx");
            _service.ExportExcelTemplateStruct(ref outFileStream, path);
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Template_Import_Activity_HopDongThauPhu.xlsx");
        }

    }
}
