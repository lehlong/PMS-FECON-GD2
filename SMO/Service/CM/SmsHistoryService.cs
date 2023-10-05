using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using System;
using System.Linq;
using NHibernate.Linq;

namespace SMO.Service.CM
{
    public class SmsHistoryService : GenericService<T_CM_SMS_HISTORY, SmsHistoryRepo>
    {
        public SmsHistoryService() : base()
        {

        }
    }
}
