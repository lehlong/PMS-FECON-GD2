using SMO.Core.Common;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Implement.PS;
using SMO.Service.PS.Models;
using SMO.Service.PS.Models.Report;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.Common
{
    public class PSService<T, TRepo> : GenericService<T, TRepo>, IPSService<T> where T : BasePSEntity where TRepo : PSRepository<T>
    {
        public override void Create()
        {
            try
            {
                if (this.CheckExist(x => x.CODE == this.ObjDetail.CODE))
                {
                    this.State = false;
                    this.ErrorMessage = "Mã dự án đã tồn tại";
                    return;
                }

                UnitOfWork.BeginTransaction();
                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                }

                this.ObjDetail = this.CurrentRepository.Create(this.ObjDetail);
                var projectStruct = (T_PS_PROJECT_STRUCT)ObjDetail;
                projectStruct.ID = Guid.NewGuid();
                projectStruct.GEN_CODE = this.ObjDetail.CODE;
                UnitOfWork.Repository<ProjectStructRepo>().Create(projectStruct);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
        protected IEnumerable<T_PS_TIME> GetProjectPeriods(IEnumerable<Guid> projectIds)
        {
            return UnitOfWork.Repository<ProjectTimeRepo>().Queryable().Where(x => projectIds.Contains(x.PROJECT_ID)).ToList();
        }
        protected IEnumerable<T_PS_TIME> GetAllInPeriodTimes(BaseReportModel model, IEnumerable<Guid> projectIds)
        {
            var projectPeriods = GetProjectPeriods(projectIds);
            var inPeriods = projectPeriods.Where(projectTime =>
            {
                var isStartDateInPeriod = model.FromDate >= projectTime.START_DATE && model.FromDate <= projectTime.FINISH_DATE;
                var isEndDateInPeriod = model.ToDate >= projectTime.START_DATE && model.ToDate <= projectTime.FINISH_DATE;
                var isPeriodInsideRange = model.ToDate >= projectTime.FINISH_DATE && model.FromDate <= projectTime.START_DATE;

                return isEndDateInPeriod || isStartDateInPeriod || isPeriodInsideRange;
            });
            return inPeriods;
        }

        protected IEnumerable<T_PS_TIME> GetAllBeforePeriodTimes(BaseReportModel model, IEnumerable<Guid> projectIds)
        {
            DateTime endMonth = new DateTime(model.ToDate.Value.Year, model.ToDate.Value.Month, DateTime.DaysInMonth(model.ToDate.Value.Year, model.ToDate.Value.Month));
            var projectPeriods = GetProjectPeriods(projectIds);
            return projectPeriods.Where(x => model.FromDate <= x.FINISH_DATE && model.ToDate >= x.FINISH_DATE || model.ToDate >= x.FINISH_DATE || x.FINISH_DATE == endMonth);
                
        }
        protected IEnumerable<T_PS_TIME> GetInYearPeriodTimes(BaseReportModel model, IEnumerable<Guid> projectIds)
        {
            var year = model.ToDate?.Year;
            var projectPeriods = GetProjectPeriods(projectIds);
            return projectPeriods.Where(x => x.YEAR == year);
        }
    }
}
