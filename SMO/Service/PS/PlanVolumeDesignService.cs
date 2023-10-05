using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using SMO.Service.PS.Models;

using System;
using System.Linq;

namespace SMO.Service.PS
{
    public class PlanVolumeDesignService : GenericService<T_PS_PLAN_VOLUME_DESIGN, PlanVolumeDesignRepo>
    {
        internal void UpdateValue(UpdatePlanDesignValueModel model)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var currentObj = CurrentRepository.Queryable()
                    .Where(x => x.PROJECT_ID == model.ProjectId
                    && x.PROJECT_STRUCT_ID == model.ProjectStructId)
                    .FirstOrDefault();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (currentObj != null)
                {
                    CurrentRepository.Detach(currentObj);
                    currentObj.VALUE = model.Value;
                    currentObj.UPDATE_BY = currentUser;
                    CurrentRepository.Update(currentObj);
                }
                else
                {
                    var contractDetail = UnitOfWork.Repository<ContractDetailRepo>().Queryable()
                                    .Where(x => x.Contract.PROJECT_ID == model.ProjectId && x.PROJECT_STRUCT_ID == model.ProjectStructId)
                                    .FirstOrDefault();

                    CurrentRepository.Create(new T_PS_PLAN_VOLUME_DESIGN
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = model.ProjectId,
                        PROJECT_STRUCT_ID = model.ProjectStructId,
                        CONTRACT_DETAIL_ID = contractDetail?.ID,
                        CREATE_BY = currentUser,
                        VALUE = model.Value
                    });
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
