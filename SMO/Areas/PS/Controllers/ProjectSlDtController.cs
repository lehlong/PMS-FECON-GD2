using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using SMO.Service.PS;
using SMO.Service.PS.Models;
using SMO.Service.PS.Models.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class ProjectSlDtController : Controller
    {
        private readonly ProjectSlDtService _service;

        public ProjectSlDtController()
        {
            _service = new ProjectSlDtService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Edit(Guid projectId)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.Search();
            ViewBag.ProjectTimes = _service.GetProjectTime();

            ViewBag.HanBaoHanh = _service.GetHanBaoHanh(projectId);
            ViewBag.NgayQuyetToan = _service.GetNgayQuyetToan(projectId);

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Detail(Guid projectId)
        {
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.Search();
            return PartialView(_service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateValue(UpdateValueModel model)
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

        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelKHDoanhThuDongTien(string header, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            dynamic dataheader = JArray.Parse(header);
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 2,
                
                ColumnsPercentage = new int[] { },
                ColumnsVolume = new int[] { },
                ColumnsNormal = new int[] { }
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
                var fileName = $"{dataheader[1].ToString()}_KeHoachDoanhThu_DongTien";
                var projectId = Guid.Parse(dataheader[2].ToString());

                MemoryStream outFileHeaderStream = new MemoryStream();

                _service.SetHeaderExportExcel(ref outFileHeaderStream, outFileStream, projectId, startCol: 4);
                return File(outFileHeaderStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }
        }
    }
}
