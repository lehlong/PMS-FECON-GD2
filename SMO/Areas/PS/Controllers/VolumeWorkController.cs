using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SMO.Service.PS;
using SMO.Service.PS.Models;
using SMO.Service.PS.Models.Report;
using SMO.Service.PS.Models.Report.VendorVolume;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace SMO.Areas.PS.Controllers
{
    public class VolumeWorkController : Controller
    {
        private readonly VolumeWorkService _service;

        public VolumeWorkController()
        {
            _service = new VolumeWorkService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult UpdateTimesIndex(Guid projectId, bool isCustomer, string partnerCode)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.ObjDetail.VENDOR_CODE = isCustomer ? null : partnerCode;

            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult UpdateTimesIndexCus(Guid projectId, bool isCustomer, string partnerCode)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.ObjDetail.VENDOR_CODE = isCustomer ? null : partnerCode;

            return PartialView(_service);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult UpdateTimesList(VolumeWorkService service)
        {
            service.Search();
            ViewBag.WorkDetail = _service.GetAllVolumeWorkDetail();
            return PartialView(service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult Index(Guid projectId, bool isCustomer, string modulName)
        {
            ViewBag.ModulName = modulName;
            var _service = new VolumeWorkService();

            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        [MyValidateAntiForgeryToken]
        public ActionResult IndexView(Guid projectId, bool isCustomer)
        {
            var _service = new VolumeWorkService();

            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            return PartialView(_service);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult IndexVolumeWork(Guid id, Guid projectId, bool isCustomer, string partnerCode)
        {
            if (id != Guid.Empty)
            {
                _service.Get(id);
            }
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;
            _service.ObjDetail.VENDOR_CODE = isCustomer ? null : partnerCode;
            _service.ObjDetail.ID = id;
            
            return PartialView(_service);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult IndexVolumeWorkView(Guid projectId, bool isCustomer)
        {
            _service.ObjDetail.IS_CUSTOMER = isCustomer;
            _service.ObjDetail.PROJECT_ID = projectId;

            return PartialView(_service);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult EditVolumeWorkCustomer(VolumeWorkService service)
        {
            var data = service.GetPlanProgresses();
            ViewBag.CurrentObj = service.ObjDetail;
            ViewBag.Customer = service.GetCustomer(service.ObjDetail.PROJECT_ID);
            ViewBag.ViewId = service.ViewId;
            return PartialView(data);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult EditVolumeWorkVendor(VolumeWorkService service)
        {
            if (!service.ObjDetail.IS_CUSTOMER)
            {
                ViewBag.Vendor = service.GetVendor(service.ObjDetail.VENDOR_CODE);
            }
            var data = service.GetPlanProgresses();
            ViewBag.CurrentObj = service.ObjDetail;
            ViewBag.ViewId = service.ViewId;
            return PartialView(data);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult VolumeWorkCustomerView(VolumeWorkService service)
        {
            var data = service.GetPlanProgresses();
            return PartialView(data);
        }

        //[MyValidateAntiForgeryToken]
        public ActionResult VolumeWorkVendorView(VolumeWorkService service)
        {
            var data = service.GetPlanProgresses();
            return PartialView(data);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult UpdateStatus(UpdateStatusVolumeModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            var updateStatus = _service.UpdateStatus(model, isAccept: false);
            if (_service.State)
            {
                result.ExtData = $"refreshProjectWorkVolumeStatusData(); SubmitTimesIndex(); $('#sapDocId').val('{_service.ObjDetail.SAP_DOCID}')";
                SMOUtilities.GetMessage("1002", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1005", _service, result);
            }
            result.Data = updateStatus.GetValue();
            return result.ToJsonResult();
        }
        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult AddUpdateTimes(AcceptTimesModel model, string isSent, string yKien)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            var id = _service.AddUpdateTimes(model, isSent, yKien);
            if (_service.State)
            {
                result.Data = id;
                SMOUtilities.GetMessage(model.Id != Guid.Empty ? "1002" : "1001", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage(model.Id != Guid.Empty ? "1005" : "1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult RefreshData(AcceptTimesModel model)
        {
            var result = new TransferObject
            {
                Type = TransferType.AlertSuccessAndJsCommand
            };
            var id = _service.RefreshData(model);
            if (_service.State)
            {
                result.Data = id;
                SMOUtilities.GetMessage(model.Id != Guid.Empty ? "1002" : "1001", _service, result);
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage(model.Id != Guid.Empty ? "1005" : "1004", _service, result);
            }
            return result.ToJsonResult();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult ProgressButtons(Guid id)
        {
            ViewBag.RefreshData = "refreshProgressButtons();";
            _service.Get(id);
            ViewBag.IsCustomer = _service.ObjDetail.IS_CUSTOMER;
            ViewBag.Modul = "Work";
            ViewBag.ProjectId = _service.ObjDetail.PROJECT_ID;
            ViewBag.IsAccept = false;

            return PartialView($"_ProjectVolumeProgressButtons", _service.ObjDetail.STATUS);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult GetTimePeriodProjectVolumeData(RefreshTimePeriodModel model)
        {
            var data = _service.GetTimePeriodWorkVolumeData(model);
            return Json(new { _service.State, Data = data });
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelVendor(string header, string data, string toDate, string fromDate)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            dynamic dataheader = JArray.Parse(header);
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 5,
                EndColumnNumber = 13,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 3
                        }, _service.GetTextDuAn(dataheader[1].ToString())
                    },

                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 4
                        }, _service.GetTextNhaThau(dataheader[0].ToString())
                    },

                    {
                        new HeaderPosition
                        {
                            X = 9,
                            Y = 3
                        }, _service.GetTextTuNgay(dataheader[3].ToString()+"/"+dataheader[4].ToString()+"/"+dataheader[5].ToString())
                    },

                    {
                        new HeaderPosition
                        {
                            X = 9,
                            Y = 4
                        }, _service.GetTextDenNgay(dataheader[6].ToString()+"/"+dataheader[7].ToString()+"/"+dataheader[8].ToString())
                    },
                },
                ColumnsPercentage = new int[] { 13},
                ColumnsVolume = new int[] { 4, 5, 6, 7, 8, 9, 11, 12, 13, 15 },

            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Export/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = $"{dataheader[2].ToString()}_KhoiLuongThucHienThauPhu";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ExportExcelCustomer(string header, string data)
        {
            var jsonData = JsonConvert.DeserializeObject<ExportDataModel>(data);
            dynamic dataheader = JArray.Parse(header);
            var config = new ConfigTemplateModel
            {
                StartRow = 8,
                StartColumnNumber = 5,
                EndColumnNumber = 13,
                HeaderParams = new Dictionary<HeaderPosition, string>
                {
                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 3
                        }, _service.GetTextDuAn(dataheader[1].ToString())
                    },

                    {
                        new HeaderPosition
                        {
                            X = 3,
                            Y = 4
                        }, _service.GetTextKhachHang(dataheader[0].ToString())
                    },

                    {
                        new HeaderPosition
                        {
                            X = 9,
                            Y = 3
                        }, _service.GetTextTuNgay(dataheader[3].ToString()+"/"+dataheader[4].ToString()+"/"+dataheader[5].ToString())
                    },

                    {
                        new HeaderPosition
                        {
                            X = 9,
                            Y = 4
                        }, _service.GetTextDenNgay(dataheader[6].ToString()+"/"+dataheader[7].ToString()+"/"+dataheader[8].ToString())
                    },
                },
                ColumnsPercentage = new int[] {13},
                ColumnsVolume = new int[] { 4, 5, 6, 7, 8, 9, 10, 11, 12 },

            };
            MemoryStream outFileStream = new MemoryStream();
            var path = Server.MapPath("~/TemplateExcel/Export/" + jsonData.Name);
            _service.ExportReportExcel(ref outFileStream, path, config, jsonData.Data, jsonData.Header);
            var fileName = $"{dataheader[2].ToString()}_KhoiLuongThucHienKhachHang";
            if (!_service.State)
            {
                return Content(_service.ErrorMessage);
            }
            return File(outFileStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
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
                result.ExtData = "SubmitTimesIndex();";
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
