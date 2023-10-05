using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class UserHistoryService : GenericService<T_AD_USER_HISTORY, UserHistoryRepo>
    {
        public UserHistoryService() : base()
        {

        }
    }
}
