using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMO.Repository.Implement.PS;
using SMO.Service.PS;
using SMO.Service.PS.Models;
using SMO.Service.PS.Models.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class PlanCostController : Controller
    {
        private readonly PlanCostService _service;

        public PlanCostController()
        {
            _service = new PlanCostService();
        }
        [MyValidateAntiForgeryToken]
        public ActionResult IndexPlanCost(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexPlanCostView(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult EditPlanCostVendor(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            var data = _service.GetPlanCosts();
            ViewBag.ProjectTimes = _service.GetProjectTime();
            ViewBag.ProjectId = projectId;
            ViewBag.HanBaoHanh = _service.UnitOfWork.Repository<ProjectRepo>().Queryable().FirstOrDefault(x => x.ID == projectId)?.HAN_BAO_HANH;
            ViewBag.NgayQuyetToan = _service.UnitOfWork.Repository<ProjectRepo>().Queryable().FirstOrDefault(x => x.ID == projectId)?.NGAY_QUYET_TOAN;

            return PartialView(data);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult EditPlanCostCustomer(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            var data = _service.GetPlanCosts();
            ViewBag.ProjectTimes = _service.GetProjectTime();
            ViewBag.ProjectId = projectId;

            ViewBag.HanBaoHanh = _service.UnitOfWork.Repository<ProjectRepo>().Queryable().FirstOrDefault(x => x.ID == projectId)?.HAN_BAO_HANH;
            ViewBag.NgayQuyetToan = _service.UnitOfWork.Repository<ProjectRepo>().Queryable().FirstOrDefault(x => x.ID == projectId)?.NGAY_QUYET_TOAN;

            return PartialView(data);
        }

        [HttpPost]
        //[MyValidateAntiForgeryToken]
        public ActionResult PlanCostVendorView(PlanCostService service)
        {
            var data = service.GetPlanCosts();
            ViewBag.ProjectTimes = service.GetProjectTime();
            ViewBag.ProjectId = service.ObjDetail.PROJECT_ID;

            return PartialView(data);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult PlanCostCustomerView(PlanCostService service)
        {
            var data = service.GetPlanCosts();
            ViewBag.ProjectTimes = service.GetProjectTime();
            ViewBag.ProjectId = service.ObjDetail.PROJECT_ID;

            return PartialView(data);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateValueCustomer(UpdatePlanCostValueModel model)
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

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateValueVendor(UpdatePlanCostValueModel model)
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
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateParentTotal(UpdatePlanCostValueModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            _service.UpdateParentTotal(model);
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

        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelKHSanLuong(string header, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            dynamic dataheader = JArray.Parse(header);
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 4,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 3
                        }, _service.GetTextDuAn(dataheader[0].ToString())
                    },
                },
                ColumnsPercentage = new int[] { },
                ColumnsVolume = new int[] { },
                ColumnsNormal = new int[] {9,10}
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Export/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            } else
            {
                var fileName = $"{dataheader[1].ToString()}_KeHoachSanLuong";
                var projectId = Guid.Parse(dataheader[2].ToString());

                MemoryStream outFileHeaderStream = new MemoryStream();

                _service.SetHeaderExportExcel(ref outFileHeaderStream, outFileStream, projectId, startCol: 12);
                return File(outFileHeaderStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }
        }

        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelKHChiPhi(string header, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            dynamic dataheader = JArray.Parse(header);
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 4,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 3
                        }, _service.GetTextDuAn(dataheader[0].ToString())
                    },

                },
                ColumnsPercentage = new int[] { },
                ColumnsVolume = new int[] { },
                ColumnsNormal = new int[] { 8, 9 }
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Export/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            else
            {
                var fileName = $"{dataheader[1].ToString()}_KeHoachChiPhi";
                var projectId = Guid.Parse(dataheader[2].ToString());

                MemoryStream outFileHeaderStream = new MemoryStream();

                _service.SetHeaderExportExcel(ref outFileHeaderStream, outFileStream, projectId, startCol: 12);
                return File(outFileHeaderStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }
        }

    }
}
