using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;
using System.Collections.Generic;
using System.Linq;
using System;
using NHibernate.Linq;

namespace SMO.Repository.Implement.CM
{
    public class FileUploadRepo : GenericRepository<T_CM_FILE_UPLOAD>, IFileUploadRepo
    {
        public FileUploadRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
    }
}
