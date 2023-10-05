using Newtonsoft.Json;
using SMO.Repository.Implement.PS;
using SMO.Service.PS;
using SMO.Service.PS.Models;
using SMO.Service.PS.Models.Report;
using SMO.Service.PS.Models.Report.CustomerContract;
using SMO.Service.PS.Models.Report.ProjectCostControl;
using SMO.Service.PS.Models.Report.ProjectDetailDataCost;
using SMO.Service.PS.Models.Report.ResourceReport;
using SMO.Service.PS.Models.Report.SummaryProject;
using SMO.Service.PS.Models.Report.VendorMonitoring;
using SMO.Service.PS.Models.Report.VendorVolume;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class ReportController : Controller
    {
        private readonly ProjectService _service;
        public ReportController()
        {
            _service = new ProjectService();
        }

        #region Báo cáo khối lượng thầu phụ
        [MyValidateAntiForgeryToken]
        public ActionResult VendorVolumeIndex(Guid? projectId)
        {
            var model = new VendorVolumeReportModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;
                model.DepartmentId = project?.PHONG_BAN;
                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Vendors = _service.GetVendors();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorVolumeReport(VendorVolumeReportModel model)
        {
            var reportData = _service.GenerateVendorVolumeReport(model);
            ViewBag.SearchModel = model;
            return PartialView(reportData);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelVendorVolume(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<VendorVolumeReportModel>(searchModel);
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 5,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 3
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 4
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 5,
                            Y = 3
                        }, _service.GetDepartmentTextExportExcel(model.DepartmentId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 5,
                            Y = 4
                        }, _service.GetVendorTextExportExcel(model.Vendor)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 10,
                            Y = 3
                        }, _service.GetStatusTextExportExcel(model.Status)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 10,
                            Y = 4
                        }, $"Từ ngày: {model.FromDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 4
                        }, $"Đến ngày: {model.ToDate?.ToString(Global.DateToStringFormat)}"
                    },
                },
                ColumnsPercentage = new int[] { 11, 15, 17, 18 },
                ColumnsVolume = new int[] { 4, 5, 6, 7, 8, 9, 11, 12, 13, 15 },
                BoldRowIndexes = model.BoldRowIndexes,
                PercentageRowIndexes = model.PercentageRowIndexes
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = $"{model.FromDate:yyyyMMMdd}_{model.ToDate:yyyyMMMdd}_BC_KHOI_LUONG_THAU_PHU";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        #endregion

        #region Xuất dữ liệu chi tiết dự án (Chi phí)
        [MyValidateAntiForgeryToken]
        public ActionResult ProjectDetailDataCostIndex(Guid? projectId)
        {
            var model = new ProjectDetailDataCostModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;
                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Vendors = _service.GetVendors();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectDetailDataCostReport(ProjectDetailDataCostModel model)
        {
            var reportData = _service.GenerateProjectDetailDataReport(model, isCustomer: false);
            ViewBag.SearchModel = model;
            return PartialView(reportData);
        }

        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelProjectDetailDataCost(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<VendorVolumeReportModel>(searchModel);
            var columnsPercentage = jsonData.Data.FirstOrDefault() == null ? new int[0] : new int[(jsonData.Data.First().Count() - 5) / 7];
            var columnsVolume = jsonData.Data.FirstOrDefault() == null ? new int[0] : new int[(jsonData.Data.First().Count() - 5) * 3 / 7];
            for (int i = 1; i <= columnsPercentage.Length; i++)
            {
                columnsPercentage[i - 1] = i * 7 + 4;
                columnsVolume[(i - 1) * 3 + 0] = (i - 1) * 7 + 4 + 1;
                columnsVolume[(i - 1) * 3 + 1] = (i - 1) * 7 + 4 + 3;
                columnsVolume[(i - 1) * 3 + 2] = (i - 1) * 7 + 4 + 5;
            }
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 5,
                BoldRowIndexes = model.BoldRowIndexes,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 4,
                            Y = 3
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 3
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 4,
                            Y = 4
                        }, _service.GetVendorTextExportExcel(model.Vendor)
                    },
                },
                ColumnsPercentage = columnsPercentage,
                ColumnsVolume = columnsVolume,
                PercentageRowIndexes = model.PercentageRowIndexes
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            var numberPeriods = jsonData.Data.FirstOrDefault()?.Count() > 0 ? (jsonData.Data.First().Count() - 9) / 7 : 0;
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);


            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            else
            {
                MemoryStream outFileHeaderStream = new MemoryStream();

                _service.SetHeaderExportExcel(ref outFileHeaderStream, outFileStream, numberPeriods, model, 11);
                var fileName = $"XUAT_DU_LIEU_CHI_TIET_DU_AN_CHI_PHI";
                return File(outFileHeaderStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }
        }
        #endregion

        #region Báo cáo thực hiện hợp đồng khách hàng
        [MyValidateAntiForgeryToken]
        public ActionResult CustomerContractIndex(Guid? projectId)
        {
            var model = new CustomerContractReportModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;
                model.ProjectLevel = project?.PROJECT_LEVEL_CODE;
                model.DepartmentId = project?.PHONG_BAN;
                model.GiamDocDuAn = project?.GIAM_DOC_DU_AN;
                //model.Status = project?.STATUS.GetEnum<ProjectStatus>();
                model.Customer = project?.CUSTOMER_CODE;
                model.ProjectType = project?.TYPE;
                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Customers = _service.GetCustomers();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CustomerContractReport(CustomerContractReportModel model)
        {
            var reportData = _service.GenerateCustomerContractReport(model);
            ViewBag.SearchModel = model;
            return PartialView(reportData);
        }

        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportCustomerContract(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<CustomerContractReportModel>(searchModel);
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
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 4
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 3
                        }, _service.GetDepartmentTextExportExcel(model.DepartmentId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 4
                        }, _service.GetCustomerTextExportExcel(model.Customer)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 3
                        }, _service.GetLeaderTextExportExcel(model.GiamDocDuAn)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 4
                        }, $"Từ ngày: {model.FromDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 3
                        }, _service.GetProjectTypeTextExportExcel(model.ProjectType)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 4
                        }, $"Đến ngày: {model.ToDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 3
                        }, _service.GetProjectLevelTextExportExcel(model.ProjectLevel)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 4
                        }, _service.GetStatusTextExportExcel(model.Status)
                    },
                },
                ColumnsPercentage = new int[] { 20, 27, 29, 30 },
                ColumnsVolume = new int[] { 5, 7, 9, 11, 13, 14, 16, 18, 21, 23, 25 }
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = $"BC_THUC_HIEN_HOP_DONG_KHACH_HANG";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        #endregion

        #region Báo cáo theo dõi thầu phụ
        [MyValidateAntiForgeryToken]
        public ActionResult VendorMonitoringIndex(Guid? projectId)
        {
            var model = new VendorMonitoringReportModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;
                model.ProjectLevel = project?.PROJECT_LEVEL_CODE;
                model.DepartmentId = project?.PHONG_BAN;
                model.GiamDocDuAn = project?.GIAM_DOC_DU_AN;
                model.ProjectType = project?.TYPE;
                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Vendors = _service.GetVendors();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VendorMonitoringReport(VendorMonitoringReportModel model)
        {
            var reportData = _service.GenerateVendorMonitoringReport(model);
            ViewBag.SearchModel = model;
            return PartialView(reportData);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelVendorMonitoring(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<VendorMonitoringReportModel>(searchModel);

            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 5,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 3
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 4
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 6,
                            Y = 3
                        }, _service.GetDepartmentTextExportExcel(model.DepartmentId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 6,
                            Y = 4
                        }, _service.GetVendorTextExportExcel(model.Vendor)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 3
                        }, _service.GetLeaderTextExportExcel(model.GiamDocDuAn)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 4
                        }, $"Từ ngày: {model.FromDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 3
                        }, _service.GetProjectTypeTextExportExcel(model.ProjectType)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 4
                        }, $"Đến ngày: {model.ToDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 3
                        }, _service.GetProjectLevelTextExportExcel(model.ProjectLevel)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 4
                        }, _service.GetStatusTextExportExcel(model.Status)
                    },
                },
                ColumnsPercentage = new int[] { 14, 21, 23, 24 },
                BoldRowIndexes = model.BoldRowIndexes,
                ColumnsVolume = new int[] { 6, 8, 10, 12, 15, 17, 19 },
                PercentageRowIndexes = model.PercentageRowIndexes
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = $"BC_THEO_DOI_THAU_PHU";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        #endregion

        #region Xuất dữ liệu chi tiết dự án (BOQ)
        [MyValidateAntiForgeryToken]
        public ActionResult ProjectDetailDataBoqIndex(Guid? projectId)
        {
            var model = new ProjectDetailDataCostModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;
                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Customers = _service.GetCustomers();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectDetailDataBoqReport(ProjectDetailDataCostModel model)
        {
            var reportData = _service.GenerateProjectDetailDataReport(model, isCustomer: true);
            ViewBag.SearchModel = model;
            return PartialView(reportData);
        }

        [HttpPost, ValidateInput(false)]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelProjectDetailDataBoq(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<ProjectDetailDataCostModel>(searchModel);
            var columnsPercentage = jsonData.Data.FirstOrDefault() == null ? new int[0] : new int[(jsonData.Data.First().Count() - 4) / 7];
            var columnsVolume = jsonData.Data.FirstOrDefault() == null ? new int[0] : new int[(jsonData.Data.First().Count() - 5) * 3 / 7];
            for (int i = 1; i <= columnsPercentage.Length; i++)
            {
                columnsPercentage[i - 1] = i * 7 + 3;
                columnsVolume[(i - 1) * 3 + 0] = (i - 1) * 7 + 3 + 1;
                columnsVolume[(i - 1) * 3 + 1] = (i - 1) * 7 + 3 + 3;
            }
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 4,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 4,
                            Y = 3
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 3
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 4,
                            Y = 4
                        }, _service.GetContractTextExportExcel(model.ContractId)
                    },
                },
                ColumnsPercentage = columnsPercentage,
                ColumnsVolume = columnsVolume,
                BoldRowIndexes = model.BoldRowIndexes,
                PercentageRowIndexes = model.PercentageRowIndexes
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            var numberPeriods = jsonData.Data.FirstOrDefault()?.Count() > 0 ? (jsonData.Data.First().Count() - 9) / 7 : 0;
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);


            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            else
            {
                MemoryStream outFileHeaderStream = new MemoryStream();

                _service.SetHeaderExportExcel(ref outFileHeaderStream, outFileStream, numberPeriods, model, startCol: 10);
                var fileName = $"XUAT_DU_LIEU_CHI_TIET_DU_AN_BOQ";
                return File(outFileHeaderStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }
        }
        #endregion

        #region Báo cáo tổng hợp dự án
        [MyValidateAntiForgeryToken]
        public ActionResult SummaryProjectIndex(Guid? projectId)
        {
            var model = new SummaryProjectReportModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;
                model.ProjectLevel = project?.PROJECT_LEVEL_CODE;
                model.DepartmentId = project?.PHONG_BAN;
                model.GiamDocDuAn = project?.GIAM_DOC_DU_AN;
                //model.Status = project?.STATUS.GetEnum<ProjectStatus>();
                model.Customer = project?.CUSTOMER_CODE;
                model.ProjectType = project?.TYPE;
                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Customers = _service.GetCustomers();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SummaryProjectReport(SummaryProjectReportModel model)
        {
            var reportData = _service.GenerateSummaryProjectReport(model);
            ViewBag.SearchModel = model;
            return PartialView(reportData);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelSummaryProject(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<SummaryProjectReportModel>(searchModel);
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 5,
                BoldRowIndexes = { 0 },
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 3
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 4
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 3
                        }, _service.GetDepartmentTextExportExcel(model.DepartmentId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 4
                        }, _service.GetCustomerTextExportExcel(model.Customer)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 3
                        }, _service.GetLeaderTextExportExcel(model.GiamDocDuAn)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 4
                        }, $"Từ ngày: {model.FromDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 3
                        }, _service.GetProjectTypeTextExportExcel(model.ProjectType)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 4
                        }, $"Đến ngày: {model.ToDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 3
                        }, _service.GetProjectLevelTextExportExcel(model.ProjectLevel)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 4
                        }, _service.GetStatusTextExportExcel(model.Status)
                    },
                },
                ColumnsPercentage = new int[] { 34, 35 },
                EndColumnNumber = 35
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = "BC_TONG_HOP_DU_AN";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        #endregion

        #region Báo cáo kiểm soát chi phí dự án
        [MyValidateAntiForgeryToken]
        public ActionResult ProjectCostControlIndex(Guid? projectId)
        {
            var model = new ProjectCostControlReportModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;
                model.ProjectLevel = project?.PROJECT_LEVEL_CODE;
                model.DepartmentId = project?.PHONG_BAN;
                model.GiamDocDuAn = project?.GIAM_DOC_DU_AN;
                //model.Status = project?.STATUS.GetEnum<ProjectStatus>();
                model.ProjectType = project?.TYPE;
                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Vendors = _service.GetVendors();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectCostControlReport(ProjectCostControlReportModel model)
        {
            var reportData = _service.GenerateProjectCostControlReport(model);
            ViewBag.SearchModel = model;
            return PartialView(reportData.OrderBy(x => x.ProjectCode).ThenBy(x => x.Order));
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelProjectCostControl(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<ProjectCostControlReportModel>(searchModel);
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
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 4
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 3
                        }, _service.GetDepartmentTextExportExcel(model.DepartmentId)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 4
                        }, _service.GetVendorTextExportExcel(model.Vendor)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 3
                        }, _service.GetLeaderTextExportExcel(model.GiamDocDuAn)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 12,
                            Y = 4
                        }, $"Từ ngày: {model.FromDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 3
                        }, _service.GetProjectTypeTextExportExcel(model.ProjectType)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 15,
                            Y = 4
                        }, $"Đến ngày: {model.ToDate?.ToString(Global.DateToStringFormat)}"
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 3
                        }, _service.GetProjectLevelTextExportExcel(model.ProjectLevel)
                    },
                    {
                        new HeaderPosition
                        {
                            X = 18,
                            Y = 4
                        }, _service.GetStatusTextExportExcel(model.Status)
                    },
                },
                ColumnsPercentage = new int[] { 12, 19 },
                BoldRowIndexes = model.BoldRowIndexes,
                ColumnsVolume = new int[] { 5, 8, 13, 15 },
                PercentageRowIndexes = model.PercentageRowIndexes
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = $"BC_KIEM_SOAT_CHI_PHI_DU_AN";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        #endregion 

        #region Báo cáo nhân sự
        [MyValidateAntiForgeryToken]
        public ActionResult ProjectResourceIndex(Guid? projectId)
        {
            var model = new ProjectResourceModel();
            if (projectId.HasValue && projectId != Guid.Empty)
            {
                var project = _service.GetProject(projectId.Value);

                model.ProjectId = projectId.Value;

                model.CompanyId = project?.DON_VI;
            }
            ViewBag.Vendors = _service.GetVendors();
            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectResourceReport(ProjectResourceModel model)
        {
            var data = _service.GenerateProjectResourceData(model);
            ViewBag.Config = _service.UnitOfWork.Repository<ConfigHideColumnRepo>().Queryable().FirstOrDefault(x=> x.USER_NAME == ProfileUtilities.User.USER_NAME && x.TYPE_DISPLAY == ConfigHideColumn.BAO_CAO_DANH_SACH_NHAN_SU_DU_AN.GetValue());
            ViewBag.SearchModel = model;
            return PartialView(data);
        }
        [HttpPost]
        public ActionResult ExportExcelProjectResource(string searchModel, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            var model = JsonConvert.DeserializeObject<ProjectResourceModel>(searchModel);

            var config = new ConfigTemplateModel
            {
                StartRow = 10,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 1,
                            Y = 5
                        }, _service.GetCompanyTextExportExcel(model.CompanyId)
                    },
                     {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 5
                        }, _service.GetProjectTextExportExcel(model.ProjectId)
                    },
                     {
                        new HeaderPosition
                        {
                            X = 5,
                            Y = 5
                        }, model.TypeResource == "FECON" ? "Phân nhóm: Nhân sự Fecon" : model.TypeResource == "OTHER" ? "Phân nhóm: Các bên liên quan" : "Phân nhóm:"
                    },
                      {
                        new HeaderPosition
                        {
                            X = 1,
                            Y = 6
                        }, $"Từ ngày: {model.FromDate?.ToString(Global.DateToStringFormat)}"
                    },
                      {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 6
                        }, $"Đến ngày: {model.ToDate?.ToString(Global.DateToStringFormat)}"
                    },
                      {
                        new HeaderPosition
                        {
                            X = 5,
                            Y = 6
                        }, $"Bên liên quan: {model.ResourceOther}"
                    },
                      {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 6
                        }, $"Họ và tên: {model.Username}"
                    },
                      {
                        new HeaderPosition
                        {
                            X = 8,
                            Y = 5
                        },$"Vai trò dự án: {model.Role}"
                    },

                },
                ColumnsPercentage = new int[] { },
                BoldRowIndexes = model.BoldRowIndexes,
                ColumnsVolume = new int[] { },
                PercentageRowIndexes = model.PercentageRowIndexes
            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Template-Report/" + jsonData.Name);
            _service.ExportResourceExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = $"BaoCaoNhanSu";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }
        #endregion
    }

}
