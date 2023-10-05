using Microsoft.AspNet.SignalR;
using Microsoft.Exchange.WebServices.Data;
using Newtonsoft.Json;
using NHibernate.Proxy;
using SMO.AppCode.Class;
using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using SMO.Hubs;
using SMO.Repository.Common;
using SMO.Repository.Implement.PS;
using SMO.Service;
using SMO.Service.AD;
using SMO.Service.CF;
using SMO.Service.CM;
using SMO.Service.MD;
using SMO.SMS_GW;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;

namespace SMO
{
    public class SMOUtilities
    {
        private static HttpClient httpClient = new HttpClient();

        public static List<TGBXJson> CheckTGBXAPI(string lstDoNumber)
        {
            if (string.IsNullOrWhiteSpace(lstDoNumber))
            {
                return new List<TGBXJson>();
            }
            try
            {
                var service = new SmsNotifyService();
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                if (string.IsNullOrWhiteSpace(systemConfig.ObjDetail.TGBX_API_URL) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.TGBX_API_USER_NAME) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.TGBX_API_PASSWORD))
                {
                    return new List<TGBXJson>();
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(systemConfig.ObjDetail.TGBX_API_URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var url = string.Format("api/smoapp/datachecklist");
                var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("user", systemConfig.ObjDetail.TGBX_API_USER_NAME),
                    new KeyValuePair<string, string>("password", systemConfig.ObjDetail.TGBX_API_PASSWORD),
                    new KeyValuePair<string, string>("deliveries_string", lstDoNumber)
                };

                var content = new FormUrlEncodedContent(pairs);
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                return JsonConvert.DeserializeObject<List<TGBXJson>>(response.Content.ReadAsStringAsync().Result);
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return new List<TGBXJson>();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgCode"></param>
        /// <param name="service"></param>
        /// <param name="transferObject"></param>
        /// <param name="paramObject"></param>
        public static void GetMessage(string msgCode, BaseService service, TransferObject transferObject, params object[] paramObject)
        {
            var id = "ERR" + DateTime.Now.ToString("yyMMddHHmmssFF");
            var code = msgCode;
            if (!string.IsNullOrWhiteSpace(service.MesseageCode))
            {
                code = service.MesseageCode;
            }
            transferObject.State = service.State;
            transferObject.Message.Code = code;
            transferObject.Message.Message = MessageUtilities.GetMessage(code);
            if (!string.IsNullOrWhiteSpace(transferObject.Message.Message))
            {
                transferObject.Message.Message = string.Format(transferObject.Message.Message, paramObject);
            }

            if (!string.IsNullOrWhiteSpace(service.ErrorMessage))
            {
                transferObject.Message.Detail = service.ErrorMessage + "<br/>";
            }

            if (service.Exception != null)
            {
                transferObject.Message.Detail += service.Exception.ToString();
                //transferObject.Message.Detail += "Xem chi tiết lỗi theo mã lỗi sau : " + id;
                Log.Error(id + Environment.NewLine + service.Exception.ToString());
            }
        }

        public static void GetMessage(string msgCode, TransferObject transferObject, params object[] paramObject)
        {
            transferObject.Message.Code = msgCode;
            transferObject.Message.Message = MessageUtilities.GetMessage(msgCode);
            if (!string.IsNullOrWhiteSpace(transferObject.Message.Message))
            {
                transferObject.Message.Message = string.Format(transferObject.Message.Message, paramObject);
            }
        }

        /// <summary>
        /// Lấy dữ liệu nhiều bảng, chỉ với một lần kết nối với database
        /// Params : listSelect là một List<SqlSelectMutil>. Cho nên khi khởi tạo dữ liệu phải thật cẩn thận. Dynamic object gồm 2 thuộc tính : Table, Where
        /// Khai báo như sau (chú ý : nếu có mệnh đề where thì object.Where = "CONDITION" - không cần thêm chữ WHERE ở condition)
        /// var lstSelect = new List<SqlSelectMutil>();
        /// lstSelect.Add(new SqlSelectMutil(){ Table = "T_AD_ROLE", Where = ""});
        /// lstSelect.Add(new SqlSelectMutil(){ Table = "T_AD_USER", Where = "1=1" });
        /// lstSelect.Add(new SqlSelectMutil(){ Table = "T_MD_BATCH", Where = "" });
        /// </summary>
        /// <param name="listSelect"></param>
        /// <returns>Dataset chứa các table kết quả. Các table đó có TableName = object.Table đã được truyền vào</returns> 
        public static DataSet GetMultilpleTable(List<SqlSelectMutil> listSelect)
        {
            DataSet dsResult = new DataSet();
            if (listSelect.Count > 0)
            {
                var strSelect = new System.Text.StringBuilder();
                foreach (var item in listSelect)
                {
                    if (string.IsNullOrEmpty(item.SQL))
                    {
                        var select = item.Select;
                        if (string.IsNullOrWhiteSpace(item.Select))
                        {
                            select = "*";
                        }
                        if (string.IsNullOrWhiteSpace(item.Where))
                        {
                            strSelect.Append(string.Format(" SELECT {0} FROM {1};", select, item.Table));
                        }
                        else
                        {
                            strSelect.Append(string.Format(" SELECT {0} FROM {1} WHERE {2};", select, item.Table, item.Where));
                        }
                    }
                    else
                    {
                        strSelect.Append(item.SQL);
                    }
                }

                using (SqlConnection objConn = new SqlConnection(ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString))
                {
                    SqlCommand objCmd = new SqlCommand(strSelect.ToString());
                    objCmd.Connection = objConn;
                    objCmd.CommandType = CommandType.Text;
                    try
                    {
                        objConn.Open();
                        var adapter = new SqlDataAdapter(objCmd);
                        adapter.Fill(dsResult);
                        int i = 0;
                        foreach (var item in listSelect)
                        {
                            dsResult.Tables[i].TableName = item.Table;
                            i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        objConn.Close();
                    }
                }
            }
            return dsResult;
        }

        /// <summary>
        /// Kiểm tra class is Proxy do Nhibernate tạo ra
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsProxy(object obj)
        {
            return NHibernateProxyHelper.IsProxy(obj);
        }

        public static string UpdateVehicleSap()
        {
            var service = new VehicleService();
            service.Synchronize();
            string result = "";
            if (!service.State)
            {
                result = service.Exception.ToString();
            }
            return result;
        }

        public static string CreateNotifyPO(string pkId, string poCode, string modulType, string status, string statusDate)
        {
            string strResult = "";
            string strTemplate = @"
                <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                    <div class='icon-circle {2}'>
                        <i class='material-icons'>{3}</i>
                    </div>
                    <div class='menu-info'>
                        <span>Đơn hàng {4} {5}</span>
                        <p>
                            <i class='material-icons'>access_time</i> {6}
                        </p>
                    </div>
                </a>
            ";
            strResult = string.Format(strTemplate,
                pkId,
                "/PO/" + modulType + "/Detail?id=" + poCode,
                PO_Status.GetStatusColorSaleManager(status),
                PO_Status.GetStatusIcon(status),
                poCode,
                PO_Status.GetStatusNotifyText(status),
                statusDate);
            return strResult;
        }

        public static void SendNotify(List<string> lstUserNotify)
        {
            var serviceNotify = new NotifyService();
            JsonSerializerSettings serializerSetting = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
            foreach (var user in lstUserNotify)
            {
                serviceNotify.GetNotifyOfUser(user);
                if (serviceNotify.IntCountNew > 0)
                {
                    hubContext.Clients.Group(user).RefreshNotify(JsonConvert.SerializeObject(serviceNotify, serializerSetting));
                }
            }
        }

        public static string SendSMS()
        {
            string strResult = "";
            try
            {
                var service = new SmsNotifyService();
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                if (string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_APP) || string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_PASSWORD))
                {
                    return "Chưa cấu hình hệ thống SMS";
                }


                var checkDate = DateTime.Now.AddDays(-2);
                var lstSms = service.UnitOfWork.GetSession().QueryOver<T_CM_SMS>().Where(
                        x => !x.IS_SEND &&
                        x.CREATE_DATE > checkDate
                        //x.NUMBER_RETRY < 6
                    ).OrderBy(x => x.CREATE_DATE).Asc
                    .Take(10).Skip(0)
                    .List();
                var gwSMS = new SendShortMessageFromExternal();
                foreach (var item in lstSms)
                {
                    try
                    {
                        gwSMS.SendSMS(item.PHONE_NUMBER.RemoveZeroWidthSpaces(), item.CONTENTS, systemConfig.ObjDetail.SMS_APP, systemConfig.ObjDetail.SMS_PASSWORD, item.USER_RECEIVED);
                        item.IS_SEND = true;
                    }
                    catch (Exception ex)
                    {
                        item.NUMBER_RETRY++;
                        strResult += $"SMS {item.PKID} : {item.PHONE_NUMBER} - {item.USER_RECEIVED} - {item.PO_CODE} - {item.MODUL_TYPE} bị lỗi : {ex.ToString()}";
                    }
                    service.ObjDetail = item;
                    service.Update();
                }
            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }

            return strResult;
        }

        public static bool SMS_VNPNT_CheckResult(int code, ref string message)
        {
            var result = false;
            message = code.ToString();
            switch (code)
            {
                case 1:
                    result = true;
                    break;
                case 2:
                    message = $"Mã code [{code}] --> Số điện thoại sai format";
                    break;
                case 4:
                    message = $"Mã code [{code}] --> Brandname chưa active";
                    break;
                case 15:
                    message = $"Mã code [{code}] --> Kết nối Gateway lỗi";
                    break;
                case 17:
                    message = $"Mã code [{code}] --> Template không hợp lệ";
                    break;
                case 25:
                    message = $"Mã code [{code}] --> Sai mạng, hoặc lable không hợp lệ";
                    break;
                case 100:
                    message = $"Mã code [{code}] --> Database error";
                    break;
                default:
                    message = $"Mã code [{code}] --> Lỗi không xác định";
                    break;
            }
            return result;
        }

        public static string SendSMS_VNPT()
        {
            string strResult = "";
            try
            {
                var service = new SmsNotifyService();
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                if (string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_APP) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_PASSWORD) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_BRAND_NAME) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.SMS_WEBSERVICE))
                {
                    return "Chưa cấu hình hệ thống SMS";
                }


                var checkDate = DateTime.Now.AddDays(-2);
                var lstSms = service.UnitOfWork.GetSession().QueryOver<T_CM_SMS>().Where(
                        x => !x.IS_SEND &&
                        //x.CREATE_DATE > checkDate
                        x.NUMBER_RETRY < 6
                    ).OrderBy(x => x.CREATE_DATE).Asc
                    .Take(200).Skip(0)
                    .List();


                var url = string.Format("api/sendsms");

                //var gwSMS = new SMS_VNPT_GW.bsmsws();
                //gwSMS.Url = systemConfig.ObjDetail.SMS_WEBSERVICE;
                foreach (var item in lstSms)
                {
                    httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri(systemConfig.ObjDetail.SMS_WEBSERVICE);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var smsHistoryService = new SmsHistoryService();
                    smsHistoryService.ObjDetail.PKID = Guid.NewGuid().ToString();
                    smsHistoryService.ObjDetail.TYPE = "1";
                    smsHistoryService.ObjDetail.HEADER_ID = item.PKID;
                    smsHistoryService.ObjDetail.CONTENTS = item.CONTENTS;
                    smsHistoryService.ObjDetail.PHONE = item.PHONE_NUMBER.RemoveZeroWidthSpaces();

                    var sms = new
                    {
                        username = systemConfig.ObjDetail.SMS_APP,
                        password = systemConfig.ObjDetail.SMS_PASSWORD,
                        phonenumber = item.PHONE_NUMBER.RemoveZeroWidthSpaces(),
                        brandname = systemConfig.ObjDetail.SMS_BRAND_NAME,
                        message = UtilsCore.ConvertCoDauKhongDau(item.CONTENTS),
                        type = "0"
                    };

                    try
                    {
                        var json = JsonConvert.SerializeObject(sms);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var response = httpClient.PostAsync(url, content);
                        var result = JsonConvert.DeserializeObject<SMSResult>(response.Result.Content.ReadAsStringAsync().Result);

                        var message = string.Empty;
                        var isSend = SMS_VNPNT_CheckResult(result.id, ref message);
                        item.IS_SEND = isSend;
                        smsHistoryService.ObjDetail.IS_SEND = isSend;
                        smsHistoryService.ObjDetail.MESSAGE = message + $"[{response.Result.Content.ReadAsStringAsync().Result}]";
                        if (!isSend)
                        {
                            item.NUMBER_RETRY++;
                        }
                    }
                    catch (Exception ex)
                    {
                        smsHistoryService.ObjDetail.IS_SEND = false;
                        smsHistoryService.ObjDetail.MESSAGE = ex.ToString();
                        item.NUMBER_RETRY++;
                        strResult += $"SMS {item.PKID} : {item.PHONE_NUMBER} - {item.USER_RECEIVED} - {item.PO_CODE} - {item.MODUL_TYPE} bị lỗi : {ex.ToString()}";
                    }
                    smsHistoryService.Create();
                    service.ObjDetail = item;
                    service.Update();
                }
            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }

            return strResult;
        }

        public static string SendEmail()
        {
            string strResult = "";
            try
            {
                var service = new EmailNotifyService();
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                if (string.IsNullOrWhiteSpace(systemConfig.ObjDetail.MAIL_HOST) ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.MAIL_PASSWORD) ||
                    systemConfig.ObjDetail.MAIL_PORT == 0 ||
                    string.IsNullOrWhiteSpace(systemConfig.ObjDetail.MAIL_USER))
                {
                    return "Chưa cấu hình hệ thống Email";
                }


                var checkDate = DateTime.Now.AddDays(-2);
                var lstEmail = service.UnitOfWork.GetSession().QueryOver<T_CM_EMAIL>().Where(
                        x => !x.IS_SEND &&
                        x.CREATE_DATE > checkDate
                        // && x.NUMBER_RETRY < 6
                    ).OrderBy(x => x.CREATE_DATE).Asc
                    .Take(5).Skip(0)
                    .List();
                ExchangeService _service;
                _service = new ExchangeService
                {
                    Credentials = new WebCredentials(systemConfig.ObjDetail.MAIL_USER, systemConfig.ObjDetail.MAIL_PASSWORD),
                    Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx")
                };
                _service.AutodiscoverUrl(systemConfig.ObjDetail.MAIL_USER);
                foreach (var item in lstEmail)
                {
                    try
                    {
                        EmailMessage message = new EmailMessage(_service);
                        foreach (var email in item.EMAIL.Split(new[] { ";", ",", "|" }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            message.ToRecipients.Add(email);
                        }
                        message.Subject = item.SUBJECT;
                        message.Body = item.CONTENTS;

                        message.Save();
                        message.SendAndSaveCopy();
                    }
                    catch (Exception ex)
                    {
                        item.IS_SEND = false;
                        item.NUMBER_RETRY++;
                        strResult += $"EMAIL {item.PKID} : {item.EMAIL} bị lỗi : {ex.ToString()}";
                    }
                    item.IS_SEND = true;
                    service.ObjDetail = item;
                    service.Update();
                }
            }
            catch (Exception ex)
            {
                strResult = ex.ToString();
            }

            return strResult;
        }

        public static List<TimePeriod> GenerateTimeWeek(DateTime startDate, DateTime endDate,DateTime? ngayQuyetToan, DateTime? hanBaoHanh)
        {
            var lstTime = new List<TimePeriod>();

            int days = startDate.DayOfWeek - DayOfWeek.Monday;
            DateTime tmpStartDate = startDate.AddDays(-days).Date;
            while (tmpStartDate < endDate)
            {
                var timePeriod = new TimePeriod();
                timePeriod.StartDate = tmpStartDate;
                if (tmpStartDate < startDate)
                {
                    timePeriod.StartDate = startDate;
                }

                timePeriod.EndDate = tmpStartDate.AddDays(6);
                if (tmpStartDate.AddDays(6) > endDate)
                {
                    timePeriod.EndDate = endDate;
                }

                tmpStartDate = tmpStartDate.AddDays(7);
                lstTime.Add(timePeriod);
            }

            lstTime.Add(new TimePeriod
            {
                StartDate = ngayQuyetToan ?? DateTime.Now,
                EndDate = ngayQuyetToan ?? DateTime.Now,
            });
            lstTime.Add(new TimePeriod
            {
                StartDate = hanBaoHanh ?? DateTime.Now,
                EndDate = hanBaoHanh ?? DateTime.Now,
            });

            return lstTime;
        }

        public static List<TimePeriod> GenerateTimeMonth(DateTime startDate, DateTime endDate, DateTime? ngayQuyetToan, DateTime? hanBaoHanh)
        {
            var lstTime = new List<TimePeriod>();
            var isNamNhuan = false;
            DateTime tmpStartDate = startDate;
            while (tmpStartDate < endDate)
            {
                var timePeriod = new TimePeriod();
                isNamNhuan = false;
                if (tmpStartDate.Year % 400 == 0 || (tmpStartDate.Year % 4 == 0 && tmpStartDate.Year % 100 != 0))
                {
                    isNamNhuan = true;
                }

                if (tmpStartDate.Year == endDate.Year && tmpStartDate.Month == endDate.Month)
                {
                    timePeriod.StartDate = tmpStartDate;
                    timePeriod.EndDate = endDate;
                    tmpStartDate = endDate.AddDays(1);
                }
                else
                {
                    var month = tmpStartDate.Month;
                    timePeriod.StartDate = tmpStartDate;
                    if (month == 2 && isNamNhuan)
                    {
                        timePeriod.EndDate = tmpStartDate.AddDays(29 - tmpStartDate.Day);
                        tmpStartDate = tmpStartDate.AddDays(29 - tmpStartDate.Day + 1);
                    }
                    else if (month == 2 && !isNamNhuan)
                    {
                        timePeriod.EndDate = tmpStartDate.AddDays(28 - tmpStartDate.Day);
                        tmpStartDate = tmpStartDate.AddDays(28 - tmpStartDate.Day + 1);
                    }
                    else if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
                    {
                        timePeriod.EndDate = tmpStartDate.AddDays(31 - tmpStartDate.Day);
                        tmpStartDate = tmpStartDate.AddDays(31 - tmpStartDate.Day + 1);
                    }
                    else
                    {
                        timePeriod.EndDate = tmpStartDate.AddDays(30 - tmpStartDate.Day);
                        tmpStartDate = tmpStartDate.AddDays(30 - tmpStartDate.Day + 1);
                    }
                }

                lstTime.Add(timePeriod);
            }

            lstTime.Add(new TimePeriod
            {
                StartDate = ngayQuyetToan ?? DateTime.Now,
                EndDate = ngayQuyetToan ?? DateTime.Now,
            });
            lstTime.Add(new TimePeriod
            {
                StartDate = hanBaoHanh ?? DateTime.Now,
                EndDate = hanBaoHanh ?? DateTime.Now,
            });

            return lstTime;
        }

        public static T_PS_PROJECT GetProject(Guid projectId)
        {
            IUnitOfWork unitOfWork = new NHUnitOfWork();
            return unitOfWork.Repository<ProjectRepo>().Get(projectId);
        }

        public static IEnumerable<T_AD_ORGANIZE> GetAllCompanies()
        {
            IUnitOfWork unitOfWork = new NHUnitOfWork();
            return unitOfWork.GetSession().Query<T_AD_ORGANIZE>().ToList();
        }
        public static IEnumerable<T_PS_PROJECT> GetAllProjects()
        {
            IUnitOfWork unitOfWork = new NHUnitOfWork();
            return unitOfWork.GetSession().Query<T_PS_PROJECT>().ToList();
        }
        public static IEnumerable<T_PS_CONTRACT> GetAllContracts(bool isSignedWithCustomer)
        {
            IUnitOfWork unitOfWork = new NHUnitOfWork();
            return unitOfWork.GetSession().Query<T_PS_CONTRACT>().Where(x => x.IS_SIGN_WITH_CUSTOMER == isSignedWithCustomer).ToList();
        }
        public static IEnumerable<T_AD_USER> GetUsers(Expression<Func<T_AD_USER, bool>> func)
        {
            IUnitOfWork unitOfWork = new NHUnitOfWork();
            return unitOfWork.GetSession().Query<T_AD_USER>().Where(func).ToList();
        }


        public static string BytesToSize(double bytes)
        {
            var sizes = new string[] { "Bytes", "KB", "MB", "GB", "TB" };
            if (bytes == 0) return "0 Byte";
            var i = Convert.ToInt32(Math.Floor(Math.Log(bytes) / Math.Log(1024)));
            return (Math.Round(bytes / Math.Pow(1024, i), 2)).ToString() + ' ' + sizes[i];
        }

        public static string GetIconOfFile(string ext)
        {
            ext = ext.ToLower();
            var icon = "file-48.png";
            switch (ext)
            {
                case ".xlsx":
                    icon = "excel-48.png";
                    break;
                case ".xls":
                    icon = "excel-48.png";
                    break;
                case ".docx":
                    icon = "word-48.png";
                    break;
                case ".doc":
                    icon = "word-48.png";
                    break;
                case ".pptx":
                    icon = "powerpoint-48.png";
                    break;
                case ".ppt":
                    icon = "powerpoint-48.png";
                    break;

                case ".txt":
                    icon = "txt-48.png";
                    break;

                case ".zip":
                    icon = "zip-48.png";
                    break;
                case ".7z":
                    icon = "zip-48.png";
                    break;
                case ".rar":
                    icon = "zip-48.png";
                    break;

                case ".jpg":
                    icon = "image-48.png";
                    break;
                case ".png":
                    icon = "image-48.png";
                    break;
                case ".bmp":
                    icon = "image-48.png";
                    break;
                case ".jpeg":
                    icon = "image-48.png";
                    break;

                case ".psd":
                    icon = "photoshop-48.png";
                    break;
                case ".dwg":
                    icon = "dwg-48.png";
                    break;
                case ".dxf":
                    icon = "dxf-48.png";
                    break;

                case ".pdf":
                    icon = "pdf-48.png";
                    break;

                default:
                    break;
            }
            return icon;
        }
    }
}