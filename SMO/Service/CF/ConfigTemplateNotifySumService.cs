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
    public class ConfigTemplateNotifySumService : GenericService<T_CF_TEMPLATE_NOTIFY_SUM, ConfigTemplateNotifySumRepo>
    {
        public ConfigTemplateNotifySumService() : base()
        {
        }
    }
}