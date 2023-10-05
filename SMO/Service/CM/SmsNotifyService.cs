using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using System;
using System.Linq;
using NHibernate.Linq;

namespace SMO.Service.CM
{
    public class SmsNotifyService : GenericService<T_CM_SMS, SmsRepo>
    {
        public SmsNotifyService() : base()
        {

        }

        public void Reset(string id)
        {
            this.Get(id);
            this.ObjDetail.NUMBER_RETRY = 0;
            this.Update();
        }
    }
}
