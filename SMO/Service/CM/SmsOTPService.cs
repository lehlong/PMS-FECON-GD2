using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using System;
using NHibernate.Linq;
using System.Linq;
using SMO.Service.MD;
using SMO.Service.AD;
using SMO.SMS_GW;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using SMO.AppCode.Class;

namespace SMO.Service.CM
{
    public class SmsOTPService : GenericService<T_CM_SMS_OTP, SmsOTPRepo>
    {
        public string ModulType { get; set; }
        public string ListPoSelected { get; set; }
        //Phân biệt OTP được gọi từ màn hình list hay là màn hình detail
        // = 1 --> màn hình detail
        public string IsDetail { get; set; }
        public string ViewIDDetail { get; set; }
        public string OTP_CODE { get; set; }
        public SmsOTPService() : base()
        {

        }

        public override void Create()
        {
            try
            {
                var userService = new UserService();
                userService.Get(ProfileUtilities.User.USER_NAME);
                if (string.IsNullOrWhiteSpace(userService.ObjDetail.PHONE))
                {
                    return;
                }

                this.ObjDetail.PKID = Guid.NewGuid().ToString();
                this.ObjDetail.MODUL_TYPE = this.ModulType;
                this.ObjDetail.USER_NAME = ProfileUtilities.User.USER_NAME;
                this.ObjDetail.OTP_CODE = (new Random()).Next(100000, 999999).ToString();

                var content = $"XangdauKV2 : Ma OTP SMO : {this.ObjDetail.OTP_CODE}";
                var service = new SmsNotifyService();
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                if (string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_APP) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_PASSWORD) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_BRAND_NAME) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_WEBSERVICE))
                {
                    this.State = false;
                    return;
                }

                var httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(systemConfig.ObjDetail.SMS_WEBSERVICE);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Gửi SMS OTP
                this.UnitOfWork.BeginTransaction();
                var isSend = false;
                var smsHistoryService = new SmsHistoryService();
                smsHistoryService.ObjDetail.PKID = Guid.NewGuid().ToString();
                smsHistoryService.ObjDetail.TYPE = "2";
                smsHistoryService.ObjDetail.HEADER_ID = this.ObjDetail.PKID;
                smsHistoryService.ObjDetail.CONTENTS = content;
                smsHistoryService.ObjDetail.PHONE = userService.ObjDetail.PHONE.RemoveZeroWidthSpaces();

                var sms = new
                {
                    username = systemConfig.ObjDetail.SMS_APP,
                    password = systemConfig.ObjDetail.SMS_PASSWORD,
                    phonenumber = userService.ObjDetail.PHONE.RemoveZeroWidthSpaces(),
                    brandname = systemConfig.ObjDetail.SMS_BRAND_NAME,
                    message = content,
                    type = "0"
                };
                var url = string.Format("api/sendsms");

                try
                {
                    var json = JsonConvert.SerializeObject(sms);
                    var contentSend = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = httpClient.PostAsync(url, contentSend);
                    var result = JsonConvert.DeserializeObject<SMSResult>(response.Result.Content.ReadAsStringAsync().Result);

                    var message = string.Empty;
                    isSend = SMOUtilities.SMS_VNPNT_CheckResult(result.id, ref message);
                    smsHistoryService.ObjDetail.MESSAGE = message;
                }
                catch (Exception ex)
                {
                    isSend = false;
                    this.Exception = ex;
                    smsHistoryService.ObjDetail.MESSAGE = ex.ToString();
                }
                smsHistoryService.ObjDetail.IS_SEND = isSend;
                smsHistoryService.Create();
                this.UnitOfWork.Commit();

                if (!isSend)
                {
                    this.State = false;
                    if (this.Exception == null)
                    {
                        this.Exception = new Exception(smsHistoryService.ObjDetail.MESSAGE);
                    }
                    return;
                }

                var strSQL = $"DELETE T_CM_SMS_OTP WHERE USER_NAME = '{ProfileUtilities.User.USER_NAME}' AND MODUL_TYPE = '{this.ModulType}';";
                this.UnitOfWork.BeginTransaction();

                // Xóa OTP cũ đi
                this.UnitOfWork.GetSession().CreateSQLQuery(strSQL).ExecuteUpdate();

                // Tạo OTP mới
                this.ObjDetail.EFFECT_TIME = DateTime.Now;
                this.CurrentRepository.Create(this.ObjDetail);

                this.UnitOfWork.Commit();

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
            
        }

        public void VerifyOTP()
        {
            try
            {
                var find = this.UnitOfWork.GetSession().QueryOver<T_CM_SMS_OTP>().
                    Where(x => x.USER_NAME == ProfileUtilities.User.USER_NAME
                    && x.MODUL_TYPE == this.ModulType
                    && x.OTP_CODE == this.OTP_CODE).SingleOrDefault();

                if (find == null)
                {
                    this.State = false;
                    this.ErrorMessage = "Mã OTP không chính xác!";
                    return;
                }
                else
                {
                    if (find.EFFECT_TIME.AddMinutes(2) < DateTime.Now)
                    {
                        this.State = false;
                        this.ErrorMessage = "Mã OTP đã hết hiệu lực!";
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Đã có lỗi xẩy ra trong quá trình kiểm tra mã OTP";
                this.Exception = ex;
            }
        }
    }
}
