using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using SMO.Core.Entities;
using SMO.Hubs;
using SMO.Service;
using SMO.Service.AD;
using SMO.Service.CM;
using SMO.Service.MD;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SMO.Controllers
{
    public class HomeController : Controller
    {
        [System.Web.Mvc.Authorize]
        public ActionResult Index()
        {
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            //hubContext.Clients.All.testNotify("kakalot");
            if (Request.Browser.Browser == "InternetExplorer" || Request.Browser.Browser == "IE")
            {
                return RedirectToAction("ChonTrinhDuyet");
            }

            if (ProfileUtilities.User == null || string.IsNullOrEmpty(ProfileUtilities.User.USER_NAME))
            {
                return RedirectToAction("Logout", "Authorize");
            }

            var service = new NotifyService();
            service.GetNotifyOfUser(ProfileUtilities.User.USER_NAME);
            ViewBag.NotifyService = service;

            AuthorizeService authorizeService = new AuthorizeService();
            authorizeService.GetInfoUser(ProfileUtilities.User.USER_NAME);
            if (!authorizeService.ObjUser.IS_IGNORE_USER)
            {
                ProfileUtilities.User.IS_IGNORE_USER = false;
                authorizeService.GetUserRight();
                authorizeService.GetUserProjectRight();
                if (authorizeService.ListUserRight.Select(x => x.CODE).Contains("R0"))
                {
                    ProfileUtilities.User.IS_IGNORE_USER = true;
                }
            }
            ProfileUtilities.UserRight = authorizeService.ListUserRight;
            ProfileUtilities.UserProjectRight = authorizeService.ListUserProjectRight;
            return View();
        }

        public ActionResult ChonTrinhDuyet()
        {
            return View();
        }

        public ActionResult KeepConnection()
        {
            return Content("");
        }


        public ActionResult Menu()
        {
            var service = new MenuService();
            service.GetMenuRole();
            return PartialView("MenuTree", service);
        }

        public ActionResult UnAuthorize(string auth)
        {
            var result = new TransferObject();
            result.Type = TransferType.AlertDanger;
            SMOUtilities.GetMessage("1100", result, auth);
            return result.ToJsonResult();
        }

        [AuthorizeCustom(Right = "R111")]
        public void UploadImage(HttpPostedFileWrapper upload)
        {
            try
            {
                if (upload != null)
                {
                    var supportedTypes = new[] { "png", "jpeg", "jpg" };
                    var fileExt = System.IO.Path.GetExtension(upload.FileName).Substring(1);
                    if (!supportedTypes.Contains(fileExt))
                    {
                        throw new Exception("Chỉ chấp nhận ảnh : png, jpeg, jpg!");
                    }
                    else if (upload.ContentLength > (2 * 1024 * 1024))
                    {
                        throw new Exception("Ảnh vượt quá dung lượng 2 MB!");
                    }

                    try
                    {
                        using (var img = Image.FromStream(upload.InputStream))
                        {
                            if (img.RawFormat.Equals(ImageFormat.Png) & img.RawFormat.Equals(ImageFormat.Jpeg))
                            {
                                throw new Exception("Chỉ chấp nhận ảnh : png, jpeg, jpg!");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    string imageName = upload.FileName;
                    string path = System.IO.Path.Combine(Server.MapPath("~/Content/ImageUpload"), imageName);
                    upload.SaveAs(path);
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        public ActionResult SelectImage()
        {
            var appData = Server.MapPath("~/Content/ImageUpload");
            List<string> lstUrlImage = new List<string>();
            var images = Directory.GetFiles(appData);
            foreach (var item in images)
            {
                lstUrlImage.Add(Url.Content("/Content/ImageUpload/" + Path.GetFileName(item)));
            }
            ViewBag.ListImage = lstUrlImage;
            return PartialView();
        }

        [System.Web.Mvc.Authorize]
        public ActionResult OpenPO(string modulType, string poCode)
        {
            ViewBag.Link = $"/PO/{modulType}/Detail?id={poCode}";
            return View();
        }

        [System.Web.Mvc.Authorize]
        public ActionResult OpenProject(string id)
        {
            if (Request.Browser.Browser == "InternetExplorer" || Request.Browser.Browser == "IE")
            {
                return RedirectToAction("ChonTrinhDuyet");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Authorize");
            }

            var linkDetail = $"/PS/Project/Edit?id={id}";
            ViewBag.Link = linkDetail;
            return View();
        }

        [System.Web.Mvc.Authorize]
        public ActionResult OpenVolumeWork(string id, string partnerCode, string projectId, bool isCustomer)
        {
            if (Request.Browser.Browser == "InternetExplorer" || Request.Browser.Browser == "IE")
            {
                return RedirectToAction("ChonTrinhDuyet");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Authorize");
            }

            var linkDetail = $"/PS/VolumeWork/IndexVolumeWork/{Guid.Parse(id)}?projectId={Guid.Parse(projectId)}&isCustomer={(isCustomer ? "true" : "false")}&partnerCode={partnerCode}";
            ViewBag.Link = linkDetail;
            return View();
        }

        [System.Web.Mvc.Authorize]
        public ActionResult OpenVolumeAccept(string id, string partnerCode, string projectId, bool isCustomer)
        {
            if (Request.Browser.Browser == "InternetExplorer" || Request.Browser.Browser == "IE")
            {
                return RedirectToAction("ChonTrinhDuyet");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Logout", "Authorize");
            }

            var linkDetail = $"/PS/VolumeAccept/IndexAcceptVolume/{Guid.Parse(id)}?projectId={Guid.Parse(projectId)}&isCustomer={(isCustomer ? "true" : "false")}&partnerCode={partnerCode}";
            ViewBag.Link = linkDetail;
            return View();
        }

        public ActionResult SendNotify(string lstUserName)
        {
            SMOUtilities.SendNotify(lstUserName.Split(',').ToList());
            return Content("");
        }

        public ActionResult TestSendMail()
        {
            SMOUtilities.SendEmail();
            return Content("test");
        }

        public ActionResult Dashboard()
        {
            var service = new NotifyService();
            service.GetNotifyOfUser(ProfileUtilities.User.USER_NAME);
            ViewBag.NotifyService = service;

            var ticket = new UserService().GetTicket();
            return View(model: ticket);
        }
    }
}