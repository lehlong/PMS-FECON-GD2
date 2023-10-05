using SMO.Service.CM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SMO.Areas.CM.Controllers
{
    [Authorize]
    public class SmsOTPController : Controller
    {
        private SmsOTPService _service;
        public SmsOTPController()
        {
            _service = new SmsOTPService();
        }

        [MyValidateAntiForgeryToken]
        public ActionResult CreateOTP(string modulType)
        {
            _service.ModulType = modulType;
            _service.Create();
            if (_service.State)
            {
                return new JsonNetResult()
                {
                    Data = "YES",
                    ContentType = (string)null,
                    ContentEncoding = (Encoding)null,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                return new JsonNetResult()
                {
                    Data = _service.Exception.Message,
                    ContentType = (string)null,
                    ContentEncoding = (Encoding)null,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyOTP(SmsOTPService service)
        {
            var result = new TransferObject();
            service.VerifyOTP();
            if (service.State)
            {
                result.Type = TransferType.AlertSuccessAndJsCommand;
                SMOUtilities.GetMessage("1002", service, result);
                result.ExtData = "try{{SubmitIndex();}}catch(e){{}};try{{ReloadDetail();}}catch(e){{}}; $('.modal').modal('hide')";
                if (service.IsDetail == "1")
                {
                    result.ExtData = string.Format("try{{SubmitIndex();}}catch(e){{}}; $('.modal').modal('hide'); Forms.LoadAjax({{url:'{0}', complete:function(){{Forms.Close('{1}');}}}});", Url.Action("Detail", service.ModulType, new {@area = "PO", id = service.ListPoSelected }), service.ViewIDDetail);
                }
            }
            else
            {
                result.Type = TransferType.JsCommand;
                SMOUtilities.GetMessage("1005", service, result); 
                result.ExtData = string.Format("$('#divErrorOTP').html('{0}')", service.ErrorMessage);
            }
            return result.ToJsonResult();
        }

        //[HttpPost]
        [MyValidateAntiForgeryToken] 
        public ActionResult DialogOTP(string modulType, string lstPoSelected, string isDetail, string viewIdDetail)
        {
            _service.IsDetail = isDetail; 
            _service.ViewIDDetail = viewIdDetail;
            _service.ModulType = modulType;
            _service.ListPoSelected = lstPoSelected;
            return PartialView(_service);
        }
    }
}