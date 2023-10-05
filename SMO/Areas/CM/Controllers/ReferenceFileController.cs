using SMO.Core.Entities;
using SMO.Service.CM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SMO.Areas.CM.Controllers
{
    public class ReferenceFileController : Controller
    {
        private ReferenceFileService _service;
        public ReferenceFileController(){
            _service = new ReferenceFileService();
        }
        // GET: CM/ReferenceFile
        public ActionResult Index(Guid referenceId)
        {
            _service.ObjDetail.REFERENCE_ID = referenceId;
            _service.GetListFiles();
            return PartialView(_service);
        }

        public ActionResult ListFiles(Guid referenceId, string isRemoveFile)
        {
            ViewBag.IsRemoveFile = isRemoveFile;
            _service.ObjDetail.REFERENCE_ID = referenceId;
            return PartialView(_service);
        }

        public ActionResult SearchListFiles(ReferenceFileService service, string isRemoveFile)
        {
            if (!string.IsNullOrWhiteSpace(isRemoveFile))
            {
                ViewBag.IsRemoveFile = isRemoveFile;
            }
            service.GetListFiles();
            return PartialView(service);
        }

        [HttpPost]
        [MyValidateAntiForgeryToken]
        public ActionResult Update(ReferenceFileService service, List<string> lstLink)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertSuccessAndJsCommand;
            service.Update(Request, lstLink ?? new List<string>());
            if (service.State)
            {
                SMOUtilities.GetMessage("1001", service, result);
                result.ExtData = "$('.frmListFiles').submit(); UploadFile.ListFile = []; $('#divPreviewFile').html('')";
            }
            else
            {
                result.Type = TransferType.AlertDanger;
                SMOUtilities.GetMessage("1004", service, result);
            }
            return result.ToJsonResult();
        }

        [Authorize]
        public ActionResult DownloadFile(Guid id, string isViewFile = "0")
        {
            try
            {
                var serviceFile = new FileUploadService();
                serviceFile.Get(id);

                var serviceConnection = serviceFile.UnitOfWork.GetSession().Query<T_AD_CONNECTION>().FirstOrDefault(x => x.PKID == serviceFile.ObjDetail.CONNECTION_ID); 

                var filePath = Path.Combine(serviceConnection.DIRECTORY.TrimEnd(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.DIRECTORY_PATH.TrimStart(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.FILE_NAME);
                if (!System.IO.File.Exists(filePath))
                {
                    return Content($"<script>alert('Đường dẫn tới file không tồn tại!');</script>;");
                }

                if (isViewFile == "1")
                {
                    var contentDispositionHeader = new System.Net.Mime.ContentDisposition
                    {
                        Inline = true,
                        FileName = serviceFile.ObjDetail.FILE_NAME
                    };
                    Response.Headers.Add("Content-Disposition", contentDispositionHeader.ToString());
                    var fileByte = System.IO.File.ReadAllBytes(filePath);
                    return File(fileByte, MimeMapping.GetMimeMapping(serviceFile.ObjDetail.FILE_NAME));
                }

                return File(filePath, MimeMapping.GetMimeMapping(serviceFile.ObjDetail.FILE_NAME), serviceFile.ObjDetail.FILE_OLD_NAME);
            }
            catch (Exception ex)
            {
                return Content($"<script>alert('Download file bị lỗi!');</script>;");
            }
        }

        [Authorize]
        public ActionResult ViewFileOnline(Guid id)
        {
            var serviceFile = new FileUploadService();
            serviceFile.Get(id);
            return PartialView(serviceFile);
        }

        [Authorize]
        public ActionResult MoveFileToTemp(Guid id)
        {
            var result = new TransferObject();
            result.Type = TransferType.JsCommand;
            try
            {
                var serviceFile = new FileUploadService();
                serviceFile.Get(id);
                var serviceConnection = serviceFile.UnitOfWork.GetSession().Query<T_AD_CONNECTION>().FirstOrDefault(x => x.PKID == serviceFile.ObjDetail.CONNECTION_ID);
                var filePath = Path.Combine(serviceConnection.DIRECTORY.TrimEnd(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.DIRECTORY_PATH.TrimStart(System.IO.Path.DirectorySeparatorChar), serviceFile.ObjDetail.FILE_NAME);
                if (!System.IO.File.Exists(filePath))
                {
                    result.State = false;
                    result.Message.Message = "Đường dẫn tới file không tồn tại!";
                }
                else
                {
                    var filePathTemp = Path.Combine("\\TempViewFile", serviceFile.ObjDetail.FILE_NAME);
                    if (!System.IO.File.Exists(HostingEnvironment.MapPath(filePathTemp)))
                    {
                        System.IO.File.Copy(filePath, HostingEnvironment.MapPath(filePathTemp));
                    }

                    result.State = true;
                    result.ExtData = filePathTemp.Replace('\\', '/');
                }
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message.Message = "Quá trình xem file bị lỗi!";
                result.Message.Detail = ex.ToString();
            }
            return result.ToJsonResult();
        }
    }
}