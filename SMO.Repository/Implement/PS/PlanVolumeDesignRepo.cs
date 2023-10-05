using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Implement.PS
{
    public class PlanVolumeDesignRepo : GenericRepository<T_PS_PLAN_VOLUME_DESIGN>, IPlanVolumeDesignRepo
    {
        public PlanVolumeDesignRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }

        public override IList<T_PS_PLAN_VOLUME_DESIGN> Search(T_PS_PLAN_VOLUME_DESIGN objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }

        public IList<T_PS_PLAN_VOLUME_DESIGN> GetPlanDesigns(Guid projectId)
        {
            return Queryable().Where(x => x.PROJECT_ID == projectId).ToList();
        }
    }
}
