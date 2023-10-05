using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.CF;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.CF
{
    public class ConfigTemplateNotifyService : GenericService<T_CF_TEMPLATE_NOTIFY, ConfigTemplateNotifyRepo>
    {
        public ConfigTemplateNotifyService() : base()
        {
        }

        public void GetTemplate()
        {
            this.GetAll();
            if (this.ObjList.Count == 0 && this.State)
            {
                this.ObjDetail = new T_CF_TEMPLATE_NOTIFY();
                this.ObjDetail.PKID = Guid.NewGuid().ToString();
                this.Create();
            }
            else
            {
                this.ObjDetail = this.ObjList[0];
            }
        }
    }
}