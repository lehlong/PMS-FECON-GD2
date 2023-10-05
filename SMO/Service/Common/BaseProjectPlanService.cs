using SMO.Core.Common;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Implement.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.Common
{
    public class BaseProjectPlanService<T, TRepo> : GenericService<T, TRepo>, IGenericService<T> where T : BaseProjectPlanEntity where TRepo : GenericRepository<T>
    {
        public string Vendor { get; set; }
        public string StructureName { get; set; }

        internal IList<T_PS_TIME> GetProjectTime()
        {
            return UnitOfWork.Repository<ProjectTimeRepo>().Queryable()
                        .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID)
                        .OrderBy(x => x.C_ORDER)
                        .ToList();
        }
        protected string GetProjectTimeTypeString(Guid projectId)
        {
            var project = GetProject(projectId);
            var timeType = project.TIME_TYPE;
            var timeTypeText = string.Empty;

            foreach (ProjectTimeTypeEnum time in Enum.GetValues(typeof(ProjectTimeTypeEnum)))
            {
                if (time.GetValue().Equals(timeType))
                {
                    timeTypeText = time.GetName();
                    break;
                }
            }
            return timeTypeText;
        }

        protected T_PS_PROJECT GetProject(Guid projectId)
        {
            return UnitOfWork.Repository<ProjectRepo>().Get(projectId);
        }
        internal IList<T_PS_PLAN_VOLUME_DESIGN> GetPlanDesigns()
        {
            return UnitOfWork.Repository<PlanVolumeDesignRepo>().GetPlanDesigns(ObjDetail.PROJECT_ID);
        }
        internal IList<T_PS_PROJECT_STRUCT> GetProjectStruct(IList<T_PS_CONTRACT_DETAIL> contractDetails)
        {
            var query = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID);

            if (ObjDetail.IS_CUSTOMER)
            {
                query = query.Where(x => x.TYPE == ProjectEnum.PROJECT.ToString() || x.TYPE == ProjectEnum.BOQ.ToString());
            }
            else
            {
                query = query.Where(x => x.TYPE == ProjectEnum.PROJECT.ToString()
                || x.TYPE == ProjectEnum.WBS.ToString()
                || x.TYPE == ProjectEnum.ACTIVITY.ToString());
            }

            if (!string.IsNullOrEmpty(Vendor))
            {
                var allStructs = query.ToList();

                var contractStructIds = contractDetails.Select(x => x.PROJECT_STRUCT_ID).ToList();
                query = query.Where(x => contractStructIds.Contains(x.ID));

                var structs = query.ToList();
                // build tree
                return BuildTree(structs, allStructs);

            }
            else if (!string.IsNullOrEmpty(StructureName))
            {
                var allStructs = query.ToList();
                query = query.Where(x => x.TEXT.ToLower().Contains(StructureName.ToLower()));
                var structs = query.ToList();
                return BuildTree(structs, allStructs);
            }
            else
            {
                query = query.OrderBy(x => x.C_ORDER).ThenBy(x => x.CREATE_DATE);
                return query.ToList();
            }
        }

        private IList<T_PS_PROJECT_STRUCT> BuildTree(IList<T_PS_PROJECT_STRUCT> children, IList<T_PS_PROJECT_STRUCT> allItems)
        {
            var tree = children;
            var structStack = new Stack<T_PS_PROJECT_STRUCT>(children);
            while (structStack.Count > 0)
            {
                var item = structStack.Pop();
                tree.Add(item);
                if (!item.PARENT_ID.HasValue)
                {
                    continue;
                }
                if (!tree.Any(x => x.ID == item.PARENT_ID.Value))
                {
                    structStack.Push(allItems.First(x => x.ID == item.PARENT_ID));
                }
            }
            return tree.Distinct().OrderBy(x => x.C_ORDER).ThenBy(x => x.CREATE_DATE).ToList();
        }

        

        internal IList<string> GetStatusesCanUpdate(string action)
        {
            ProjectWorkVolumeAction updateAction = ProjectWorkVolumeAction.TAO_MOI;
            foreach (ProjectWorkVolumeAction actionEnum in Enum.GetValues(typeof(ProjectWorkVolumeAction)))
            {
                if (actionEnum.GetValue().Equals(action))
                {
                    updateAction = actionEnum;
                    break;
                }
            }

            var statusesCanUpdate = new List<string>();

            switch (updateAction)
            {
                case ProjectWorkVolumeAction.XAC_NHAN:
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue());
                    break;
                case ProjectWorkVolumeAction.GUI:
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.KHOI_TAO.GetValue());
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue());
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.TU_CHOI.GetValue());
                    break;
                case ProjectWorkVolumeAction.HUY_PHE_DUYET:
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.PHE_DUYET.GetValue());
                    break;
                case ProjectWorkVolumeAction.KHONG_XAC_NHAN:
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue());
                    break;
                case ProjectWorkVolumeAction.TU_CHOI:
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.XAC_NHAN.GetValue());
                    break;
                case ProjectWorkVolumeAction.PHE_DUYET:
                    statusesCanUpdate.Add(ProjectWorkVolumeStatus.XAC_NHAN.GetValue());
                    break;
                case ProjectWorkVolumeAction.TAO_MOI:
                default:
                    break;
            }

            return statusesCanUpdate;
        }
    }
}
