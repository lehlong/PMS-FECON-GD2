using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using SMO.Service.Common;
using SMO.Service.PS.Models;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.PS
{
    public class PlanProgressService : BaseProjectPlanService<T_PS_PLAN_PROGRESS, PlanProgressRepo>
    {

        public IList<PlanProgressModel> GetPlanProgresses()
        {
            try
            {
                var currentUser = ProfileUtilities.User.USER_NAME;
                var queryContractDetail = UnitOfWork.Repository<ContractDetailRepo>().Queryable()
                                    .Where(x => x.Contract.PROJECT_ID == ObjDetail.PROJECT_ID);

                var hasFilerVendor = !string.IsNullOrWhiteSpace(Vendor);
                if (hasFilerVendor)
                {
                    queryContractDetail = queryContractDetail.Where(x => x.Contract.VENDOR_CODE.Equals(Vendor));
                }
                var contractDetails = queryContractDetail.ToList();
                var projectStructs = GetProjectStruct(contractDetails);
                base.Search();
                return (from projectStruct in projectStructs
                        let contractDetail = contractDetails.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                        select new PlanProgressModel
                        {
                            Id = projectStruct.ID,
                            Parent = projectStruct.PARENT_ID,
                            ProjectId = ObjDetail.PROJECT_ID,
                            ProjectStructId = projectStruct.ID,
                            TEXT = projectStruct.TEXT,
                            TYPE = projectStruct.TYPE,
                            UNIT_NAME = contractDetail?.Unit?.NAME,
                            CONTRACT_VALUE = contractDetail?.VOLUME,
                            START_DATE = projectStruct.START_DATE,
                            FINISH_DATE = projectStruct.FINISH_DATE,
                        }).ToList();
            }
            catch
            {
                return new List<PlanProgressModel>();
            }

        }

    }
}
