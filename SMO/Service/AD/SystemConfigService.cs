using Hangfire;
using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class SystemConfigService : GenericService<T_AD_SYSTEM_CONFIG, SystemConfigRepo>
    {
        public SystemConfigService() : base()
        {
            
        }

        public void GetConfig()
        {
            this.GetAll();
            this.ObjDetail = this.ObjList[0];
            //if (this.ObjList.Count == 0 && this.State)
            //{
            //    this.ObjDetail = new T_AD_SYSTEM_CONFIG();
            //    this.ObjDetail.PKID = Guid.NewGuid().ToString();
            //    this.Create();
            //}
            //else
            //{
            //    this.ObjDetail = this.ObjList[0];
            //}
        }

        public override void Update()
        {
            try
            {
                base.Update();
                if (this.State)
                {
                    SAPINT.SAPDestitination.Init(this.ObjDetail.SAP_HOST, this.ObjDetail.SAP_CLIENT, this.ObjDetail.SAP_NUMBER);
                    //this.ObjDetail.JOB_STATUS_SAP = this.ObjDetail.JOB_STATUS_SAP == 0 ? 5 : this.ObjDetail.JOB_STATUS_SAP;
                    //this.ObjDetail.JOB_STATUS_TGBX = this.ObjDetail.JOB_STATUS_TGBX == 0 ? 5 : this.ObjDetail.JOB_STATUS_TGBX;
                    //this.ObjDetail.JOB_SEND_EMAIL = this.ObjDetail.JOB_SEND_EMAIL == 0 ? 5 : this.ObjDetail.JOB_SEND_EMAIL;
                    //this.ObjDetail.JOB_SEND_SMS = this.ObjDetail.JOB_SEND_SMS == 0 ? 5 : this.ObjDetail.JOB_SEND_SMS;
                    //RecurringJob.AddOrUpdate("UpdateStatusSAP", () => SMOUtilities.UpdateStatusSAP(), Cron.MinuteInterval(this.ObjDetail.JOB_STATUS_SAP));
                    //RecurringJob.AddOrUpdate("UpdateStatusTGBX", () => SMOUtilities.UpdateStatusTGBX(), Cron.MinuteInterval(this.ObjDetail.JOB_STATUS_TGBX));
                    //RecurringJob.AddOrUpdate("SendEmail", () => SMOUtilities.SendEmail(), Cron.MinuteInterval(this.ObjDetail.JOB_SEND_EMAIL));
                    //RecurringJob.AddOrUpdate("SendSMS_VNPT", () => SMOUtilities.SendSMS_VNPT(), Cron.MinuteInterval(this.ObjDetail.JOB_SEND_SMS));
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
            
        }
    }
}
