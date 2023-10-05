using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using SMO.Service.Common;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.PS
{
    public class PlanQuantityService : BaseProjectPlanService<T_PS_PLAN_QUANTITY, PlanQuantityRepo>
    {
        public IList<PlanQuantityViewModel> GetPlanQuantities()
        {
            NumerRecordPerPage = int.MaxValue;
            var queryContractDetail = UnitOfWork.Repository<ContractDetailRepo>().Queryable()
                                    .Where(x => x.Contract.PROJECT_ID == ObjDetail.PROJECT_ID);

            var hasFilerVendor = !string.IsNullOrWhiteSpace(Vendor);
            if (hasFilerVendor)
            {
                queryContractDetail = queryContractDetail.Where(x => x.Contract.VENDOR_CODE.Equals(Vendor));
            }
            var contractDetails = queryContractDetail.ToList();
            var projectStructs = GetProjectStruct(contractDetails);
            var projectTimes = GetProjectTime();
            var planDesigns = GetPlanDesigns();
            var lstUnit = UnitOfWork.GetSession().Query<T_MD_UNIT>().ToList();
            base.Search();

            return (from projectStruct in projectStructs
                    let contractDetail = contractDetails.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                    from time in projectTimes
                    let currentValue = ObjList.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID && x.TIME_PERIOD_ID == time.ID)
                    let planDesign = planDesigns.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                    select new PlanQuantityViewModel
                    {
                        Id = projectStruct.ID,
                        ProjectId = ObjDetail.PROJECT_ID,
                        TimePeriodId = time.ID,
                        TimePeriodOrder = time.C_ORDER,
                        ParentId = projectStruct.PARENT_ID,
                        ProjectStructureId = projectStruct.ID,
                        ProjectStructureName = projectStruct.TEXT,
                        Quantity = projectStruct?.QUANTITY,
                        Price = projectStruct?.PRICE,
                        ThanhTien = projectStruct?.TOTAL,
                        ProjectStructureType = projectStruct.TYPE,
                        UnitName = lstUnit.FirstOrDefault(x => x.CODE == projectStruct.UNIT_CODE)?.NAME,
                        StartDate = projectStruct.START_DATE,
                        FinishDate = projectStruct.FINISH_DATE,
                        Value = currentValue?.VALUE ?? 0
                    }).ToList();
        }
        
        internal void UpdateValue(UpdatePlanQuantityValueModel model)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var currentObj = CurrentRepository.Queryable()
                    .Where(x => x.PROJECT_ID == model.ProjectId
                    && x.TIME_PERIOD_ID == model.PeriodId
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

                    CurrentRepository.Create(new T_PS_PLAN_QUANTITY
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = model.ProjectId,
                        IS_CUSTOMER = model.IsCustomer,
                        PROJECT_STRUCT_ID = model.ProjectStructId,
                        CONTRACT_DETAIL_ID = contractDetail?.ID,
                        TIME_PERIOD_ID = model.PeriodId,
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
