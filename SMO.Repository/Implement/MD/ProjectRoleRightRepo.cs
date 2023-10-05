using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Implement.MD
{
    public class ProjectRoleRightRepo : GenericRepository<T_MD_PROJECT_ROLE_RIGHT>, IProjectRoleRightRepo
    {
        public ProjectRoleRightRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

    }
}
