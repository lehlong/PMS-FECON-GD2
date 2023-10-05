using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;
using System.Collections.Generic;
using System.Linq;
using System;
using NHibernate.Linq;

namespace SMO.Repository.Implement.CM
{
    public class ReferenceFileRepo : GenericRepository<T_CM_REFERENCE_FILE>, IReferenceFileRepo
    {
        public ReferenceFileRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
