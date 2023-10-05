using System.Web.Mvc;
using SMO.Service.AD;
using System;
using SMO;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SMO.Areas.AD.Controllers
{

    public class FileModel
    {
        public string FileName { get; set; }
        public string FileSizeText { get; set; }
        public DateTime FileAccessed { get; set; }
    }
    [Authorize]
    public class LogController : Controller
    {
        public LogController()
        {
        }

        public ActionResult Index()
        {
            try
            {
                if (ProfileUtilities.User == null || ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
                {
                    return Content("");
                }
                List<FileModel> fileListModel = new List<FileModel>();
                string path = Server.MapPath("~/Logs/");
                var fileList = Directory.EnumerateFiles(path);
                var listFileName = new List<string>();
                foreach (var file in fileList)
                {
                    FileInfo f = new FileInfo(file);
                    FileModel fileModel = new FileModel();
                    fileModel.FileName = Path.GetFileName(file);
                    fileModel.FileAccessed = f.LastAccessTime;
                    fileModel.FileSizeText = (f.Length < 1024) ?
                             f.Length.ToString() + " B" : f.Length / 1024 + " KB";

                    fileListModel.Add(fileModel);
                }
                ViewBag.ListFile = fileListModel;
                return PartialView();
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        public ActionResult ViewLog(string id)
        {
            try
            {
                if (ProfileUtilities.User == null || ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
                {
                    return Content("");
                }
                string path = Server.MapPath("~/Logs/") + id;
                var lstLine = new List<string>();

                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.Default))
                    {
                        while (!sr.EndOfStream)
                        {
                            string line = sr.ReadLine();
                            lstLine.Add(line);
                        }
                    }
                }

                ViewBag.Content = lstLine;
                return PartialView();
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
            
        }

        public ActionResult Download(string id)
        {
            try
            {
                if (ProfileUtilities.User == null || ProfileUtilities.User.USER_NAME.ToUpper() != "SUPERADMIN")
                {
                    return Content("");
                }
                string path = Server.MapPath("~/Logs/") + id;
                using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    MemoryStream ms = new MemoryStream();
                    fs.CopyTo(ms);
                    var bytes = ms.ToArray();
                    ms.Dispose();
                    return File(bytes, System.Net.Mime.MediaTypeNames.Application.Octet, Path.GetFileName(path));
                }
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }
    }
}