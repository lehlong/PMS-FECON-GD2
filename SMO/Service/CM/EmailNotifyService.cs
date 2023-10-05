using SMO.Core.Entities;
using SMO.Repository.Implement.CM;
using System;
using System.Linq;
using NHibernate.Linq;

namespace SMO.Service.CM
{
    public class EmailNotifyService : GenericService<T_CM_EMAIL, EmailRepo>
    {
        public EmailNotifyService() : base()
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
