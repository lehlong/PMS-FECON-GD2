using Newtonsoft.Json;
using SMO.Core.Entities.PS;
using SMO.Models;
using SMO.Models.Config;
using SMO.Service.AD;
using SMO.Service.PS;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class ProjectController : Controller
    {
        private ProjectService _service;

        public ProjectController()
        {
            _service = new ProjectService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(string modulName)
        {
            ViewBag.ModulName = modulName;
            return PartialView(_service);
        }
        [MyValidateAntiForgeryToken]
        public ActionResult ProgressHistory(Guid projectId)
        {
            var data = _service.GetProgressHistory(projectId);
            return PartialView(data);
        }
        [MyValidateAntiForgeryToken]
        public ActionResult VolumeProgressHistory(Guid resourceId)
        {
            var data = _service.GetVolumeProgressHistory(resourceId);
            return PartialView(data);
        }

        [ValidateAntiForgeryToken]
        public ActionResult List(ProjectService service, string modulName)
        {
            ViewBag.ModulName = modulName;
            service.Search();
            return PartialView(service);
        }

        public ActionResult ListProjectHome()
        {
            _service.Search();
            _service.ObjList = _service.ObjList.Where(x => x.STATUS == ProjectStatus.KHOI_TAO.GetValue() 
            || x.STATUS == ProjectStatus.LAP_KE_HOACH.GetValue() 
            || x.STATUS == ProjectStatus.BAT_DAU.GetValue()
            ).ToList();
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Create()
        {
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult StartProject(Guid id)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ObjDetail.ID = id;
            _service.StartProject();
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; try{{onClickGeneralInformation_Edit();}}catch(e){{}};");
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
        public ActionResult UpdateProjectSAP(Guid projectId)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ObjDetail.ID = projectId;
            //_service.PostDataToSAP(projectId);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                //result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; try{{onClickGeneralInformation_Edit();}}catch(e){{}};");
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
        public ActionResult SyncProjectToSAP(Guid projectId)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ObjDetail.ID = projectId;
            _service.PostDataToSAP(projectId);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                //result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; try{{onClickGeneralInformation_Edit();}}catch(e){{}};");
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
        public ActionResult CloseProject(Guid id)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ObjDetail.ID = id;
            _service.CloseProject();
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; try{{onClickGeneralInformation_Edit();}}catch(e){{}};");
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
        public ActionResult DoneProject(Guid id)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.ObjDetail.ID = id;
            _service.DoneProject();
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; try{{onClickGeneralInformation_Edit();}}catch(e){{}};");
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
        public ActionResult Create(ProjectService service, List<string> lstLink)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            //service.ObjDetail.CODE = service.GenProjectCode();
            //if (string.IsNullOrEmpty(service.ObjDetail.CODE))
            //{
            //    result.Type = TransferType.AlertDanger;
            //    service.ErrorMessage = "Quá trình sinh mã project bị lỗi. Nguyên nhân có thể do chưa cấu hình danh mục Project Profile cho đơn vị phụ trách vừa chọn!";
            //    SMOUtilities.GetMessage("1004", service, result);
            //    return result.ToJsonResult();
            //}
            //service.ObjDetail.ID = Guid.NewGuid();
            //service.ObjDetail.STATUS = ProjectStatus.KHOI_TAO.GetValue();
            //service.ObjDetail.REFERENCE_FILE_ID = Guid.NewGuid();
            //service.ObjDetail.STATUS_STRUCT_PLAN = ProjectStructureProgressStatus.KHOI_TAO.GetValue();
            service.Create(Request, lstLink ?? new List<string>());
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = "SubmitIndex();";
                result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; Forms.LoadAjax({{url:'{0}', complete:function(){{Forms.Close('{1}');}}}});", Url.Action("Edit", new { id = service.ObjDetail.ID }), service.ViewId);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ViewBaoCao(Guid projectId)
        {
            _service.ObjDetail.ID = projectId;
            if (projectId != Guid.Empty)
            {
                _service.Get(projectId);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ViewListBaoCao(Guid projectId)
        {
            _service.ObjDetail.ID = projectId;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Plan(Guid projectId, string modulName)
        {
            ViewBag.ModulName = modulName;
            _service.ObjDetail.ID = projectId;
            if (projectId != Guid.Empty)
            {
                _service.Get(projectId);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Edit(Guid id, string modulName)
        {
            ViewBag.ModulName = modulName;
            _service.ObjDetail.ID = id;
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Detail(Guid id)
        {
            _service.ObjDetail.ID = id;
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Structure(Guid projectId, string modulName)
        {
            ViewBag.ModulName = modulName;
            if (projectId != Guid.Empty)
            {
                _service.Get(projectId);
            }
            return PartialView(_service);
        }

        public ActionResult ViewDashboard(Guid projectId)
        {
            if (projectId != Guid.Empty)
            {
                _service.Get(projectId);
            }
            var ticket = new UserService().GetTicket();
            ViewBag.Ticket = ticket;
            return PartialView(_service);
        }

        public ActionResult ViewDashboardCode(Guid projectId, string startDate, string finishDate)
        {
            if (projectId != Guid.Empty)
            {
                _service.Get(projectId);
            }
            DateTime fromDate = _service.ObjDetail.START_DATE;
            DateTime toDate = DateTime.Now;
            if (startDate != null || finishDate != null)
            {
                fromDate = DateTime.Parse(startDate);
                toDate = DateTime.Parse(finishDate);
            }
            ViewBag.CA = _service.GetCA(projectId);
            ViewBag.BAC = _service.GetBAC(projectId);
            ViewBag.WP = _service.GetWP(projectId, fromDate, toDate);
            ViewBag.WD = _service.GetWD(projectId, fromDate, toDate);
            ViewBag.ACW = _service.GetACW(projectId, fromDate, toDate);
            ViewBag.PE = _service.GetPE(projectId, fromDate, toDate);
            ViewBag.AC = _service.GetAC(projectId, fromDate, toDate);
            ViewBag.NT = _service.GetNT(projectId, fromDate, toDate);
            ViewBag.DTDK = _service.GetDoanhThuDuKien(projectId);

            ViewBag.DataDashboardBOQ = _service.GetDataDashboardBOQ(projectId);
            ViewBag.DataDashboardChiPhi = _service.GetDataDashboardChiPhi(projectId);

            ViewBag.DataCostLevel2 = _service.GetDataCostLevel2(projectId, fromDate, toDate);

            ViewBag.AllComment = _service.GetAllComment(projectId);
            ViewBag.Resource = _service.GetResourceProject(projectId);

            return PartialView(_service);
        }
        public ActionResult GetDataDashboardByTime(Guid projectId, string startDate, string finishDate)
        {
            if (projectId != Guid.Empty)
            {
                _service.Get(projectId);
            }
            DateTime fromDate = _service.ObjDetail.START_DATE;
            DateTime toDate = DateTime.Now;
            if (startDate != null || finishDate != null)
            {
                fromDate = DateTime.Parse(startDate);
                toDate = DateTime.Parse(finishDate);
            }
            ViewBag.CA = _service.GetCA(projectId);
            ViewBag.BAC = _service.GetBAC(projectId);
            ViewBag.WP = _service.GetWP(projectId, fromDate, toDate);
            ViewBag.WD = _service.GetWD(projectId, fromDate, toDate);
            ViewBag.ACW = _service.GetACW(projectId, fromDate, toDate);
            ViewBag.PE = _service.GetPE(projectId, fromDate, toDate);
            ViewBag.AC = _service.GetAC(projectId, fromDate, toDate);
            ViewBag.NT = _service.GetNT(projectId, fromDate, toDate);
            ViewBag.DTDK = _service.GetDoanhThuDuKien(projectId);

            ViewBag.DataDashboardBOQ = _service.GetDataDashboardBOQ(projectId);
            ViewBag.DataDashboardChiPhi = _service.GetDataDashboardChiPhi(projectId);

            ViewBag.DataCostLevel2 = _service.GetDataCostLevel2(projectId, fromDate, toDate);

            ViewBag.AllComment = _service.GetAllComment(projectId);
            ViewBag.Resource = _service.GetResourceProject(projectId);

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult StructureView(Guid projectId)
        {
            if (projectId != Guid.Empty)
            {
                _service.Get(projectId);
            }

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult StructureCost(Guid projectId)
        {
            var data = _service.GetProjectStructureCost(projectId);
            ViewBag.ProjectStructureBoqs = _service.GetStructures(projectId, new List<ProjectEnum>() { ProjectEnum.BOQ });
            ViewBag.ProjectId = projectId;
            return PartialView(data);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult StructureBoq(Guid projectId)
        {
            var data = _service.GetProjectStructureBoq(projectId);
            return PartialView(data);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult StructureCostView(Guid projectId)
        {
            var data = _service.GetProjectStructureCost(projectId);
            ViewBag.ProjectStructureBoqs = _service.GetStructures(projectId, new List<ProjectEnum>() { ProjectEnum.BOQ });
            ViewBag.ProjectId = projectId;
            return PartialView(data);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult StructureBoqView(Guid projectId)
        {
            var data = _service.GetProjectStructureBoq(projectId);
            return PartialView(data);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult EditGeneralInformation(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ViewGeneralInformation(Guid id)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            return PartialView(_service);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(ProjectService service)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            service.Update();
            if (service.State)
            {
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; try{{onClickGeneralInformation_Edit();}}catch(e){{}};");
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", service, result);
            }
            return result.ToJsonResult();
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult UpdateInformationStructureCost(IList<UpdateInformationStructureCostModel> data)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateInformationStructureCostModel(data);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                //result.ExtData = "SubmitIndex();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult TrinhDuyet(UpdateProjectPlanStatusModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateProjectPlanStatus(model, ProjectPlanStatus.CHO_PHE_DUYET);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = $"refreshProjectPlanStatusData();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult PheDuyet(UpdateProjectPlanStatusModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateProjectPlanStatus(model, ProjectPlanStatus.PHE_DUYET);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = $"refreshProjectPlanStatusData();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult TuChoi(UpdateProjectPlanStatusModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateProjectPlanStatus(model, ProjectPlanStatus.TU_CHOI);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = $"refreshProjectPlanStatusData();";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult HuyPheDuyet(UpdateProjectPlanStatusModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateProjectPlanStatus(model, ProjectPlanStatus.HUY_PHE_DUYET);
            if (_service.State)
            {
                SMOUtilities.GetMessage("1002", _service, result);
                result.ExtData = $"refreshProjectPlanStatusData();";
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
        public ActionResult UpdateStructureStatus(UpdateProjectStructureStatusModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateProjectStructureStatus(model);
            if (_service.State)
            {
                result.ExtData = $"onClickGeneralInformation_Edit();";
                SMOUtilities.GetMessage("1002", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            return result.ToJsonResult();
        }
        public ActionResult ProgressButtons(Guid projectId, string partnerType, ProjectPlanType planType)
        {
            ViewBag.ProjectPlanType = planType;
            ViewBag.RefreshData = "refreshProgressButtons();";
            return PartialView($"_{partnerType}ProgressButtons", projectId);
        }

        public ActionResult ProgressProjectButtons(Guid projectId)
        {
            ViewBag.RefreshData = "refreshProgressButtons();";
            return PartialView($"_StructureProgressButtons", projectId);
        }

        public ActionResult ExportExcelDataBOQ(Guid projectId)
        {
            _service.Get(projectId);
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Export_Data_CauTrucBOQ.xlsx");
            _service.ExportExcelDataBOQ(ref outFileStream, path, projectId);
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", _service.ObjDetail.CODE + "_CAY_CAU_TRUC_BOQ.xlsx");
        }

        public ActionResult ExportExcelTemplateBOQ()
        {
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/PMS_Template_Cay_du_an_BOQ.xlsx");
            _service.ExportExcelTemplateStruct(ref outFileStream, path);
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PMS_Template_Cay_du_an_BOQ.xlsx");
        }

        public ActionResult ExportExcelDataChiPhi(Guid projectId)
        {
            _service.Get(projectId);
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Export_Data_CauTrucCHIPHI.xlsx");
            _service.ExportExcelDataCHIPHI(ref outFileStream, path, projectId);
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", _service.ObjDetail.CODE + "_CAY_CAU_TRUC_CHI_PHI.xlsx");
        }

        public ActionResult ExportExcelTemplateCHIPHI()
        {
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/PMS_Template_Cay_du_an_CHIPHI.xlsx");
            _service.ExportExcelTemplateStruct(ref outFileStream, path);
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "PMS_Template_Cay_du_an_CHIPHI.xlsx");
        }

        public ActionResult DownloadTemplate(ProjectStructureType type)
        {
            var fileName = string.Empty;
            switch (type)
            {
                case ProjectStructureType.COST:
                    fileName = "PMS_Template_Cay_du_an_CHIPHI.xlsx";
                    break;
                case ProjectStructureType.BOQ:
                    fileName = "PMS_Template_Cay_du_an_BOQ.xlsx";
                    break;
                default:
                    break;
            }
            byte[] fileBytes = System.IO.File.ReadAllBytes(Server.MapPath("~/TemplateExcel/" + fileName));
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public void SaveComment(Guid projectId,string comment)
        {
            _service.SaveComment(projectId, comment);
        }
        [HttpPost]
        public void SaveFileComment()
        {
            for (int i = 0; i < Request.Files.Count; i++)
            {
                HttpPostedFileBase file = Request.Files[i];
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                System.IO.Stream fileContent = file.InputStream;

                var physicalPath = DateTime.Now.ToString("dd-MM-yyyy_HH-mm_") + fileName;

                var dirPath = Server.MapPath($"~/FileUpload/");

                if (!System.IO.Directory.Exists(dirPath))
                {
                    System.IO.Directory.CreateDirectory(dirPath);
                }
                file.SaveAs(dirPath + physicalPath);
                var filePath = "/FileUpload/" + physicalPath;

                Guid projectId = Guid.Parse(Request.Form[0]);
                _service.SaveFileComment(projectId, fileName, filePath, mimeType);
            }
        }
        [HttpPost]
        public ActionResult UpdateConfigHideColumn(ConfigHideColumnModels model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateConfigHideColumn(model);
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
