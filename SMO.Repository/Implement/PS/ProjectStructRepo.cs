using NHibernate.Linq;

using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class ProjectStructRepo : GenericRepository<T_PS_PROJECT_STRUCT>, IProjectStructRepo
    {
        public ProjectStructRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_PS_PROJECT_STRUCT> Search(T_PS_PROJECT_STRUCT objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (objFilter.PROJECT_ID != Guid.Empty)
            {
                query = query.Where(x => x.PROJECT_ID == objFilter.PROJECT_ID);
            }
            if (!string.IsNullOrEmpty(objFilter.TYPE))
            {
                var types = objFilter.TYPE.Split(',');
                query = query.Where(x => types.Contains(x.TYPE));
            }
            query = query.OrderBy(x => x.C_ORDER).ThenBy(x => x.CREATE_DATE);
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }

        public IEnumerable<T_PS_PROJECT_STRUCT> SearchWithLinkBoq(IEnumerable<Guid> projectIds, string elementName, IEnumerable<Guid> structureIdInContracts, bool hasFilterVendor)
        {
            var query = Queryable();

            query = query.Where(x => projectIds.Contains(x.PROJECT_ID) && !hasFilterVendor || (hasFilterVendor &&
            (x.TYPE == ProjectEnum.BOQ.ToString() || structureIdInContracts.Contains(x.ID) || x.TYPE == ProjectEnum.PROJECT.ToString())));
            var hasFilterName = !string.IsNullOrEmpty(elementName);
            if (hasFilterName)
            {
                query = query.Where(x => elementName.ToLower().Contains(x.TEXT.ToLower())
                || x.GEN_CODE.ToLower().Contains(elementName.ToLower())
                || x.TYPE == ProjectEnum.PROJECT.ToString());
            }
            query = query.Fetch(x => x.Wbs);
            query = query.Fetch(x => x.Activity);
            var structures = query.ToList();
            if (!hasFilterName)
            {
                return structures;
            }
            else
            {
                var boqIds = structures.Where(x => x.TYPE == ProjectEnum.BOQ.ToString()).Select(x => x.ID).ToList();
                var allBoqIdReferenceWbs = structures.Where(x => x.TYPE == ProjectEnum.WBS.ToString())
                    .Select(x => x.Wbs.BOQ_REFRENCE_ID)
                    .Where(x => x != null)
                    .ToList();
                var allBoqIdReferenceActivity = structures.Where(x => x.TYPE == ProjectEnum.ACTIVITY.ToString())
                    .Select(x => x.Activity.BOQ_REFRENCE_ID)
                    .Where(x => x != null)
                    .ToList();
                var additionStructures = Queryable().Where(x => allBoqIdReferenceWbs.Contains(x.ID)
                || allBoqIdReferenceActivity.Contains(x.ID)
                || boqIds.Contains(x.Activity.BOQ_REFRENCE_ID ?? Guid.Empty)
                || boqIds.Contains(x.Wbs.BOQ_REFRENCE_ID ?? Guid.Empty))
                    .Fetch(x => x.Activity)
                    .Fetch(x => x.Wbs)
                    .ToList();
                additionStructures.AddRange(structures);
                return additionStructures.GroupBy(x => x.ID).Select(x => x.First());
            }
        }

    }
}
