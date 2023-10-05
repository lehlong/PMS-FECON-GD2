using Glimpse.Core.Resource;
using NHibernate.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

using SharpSapRfc;
using SharpSapRfc.Plain;

using SMO.AppCode.Class;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Models;
using SMO.Models.Config;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;
using SMO.SAPINT;
using SMO.SAPINT.Class;
using SMO.Service.AD;
using SMO.Service.CF;
using SMO.Service.Class;
using SMO.Service.CM;
using SMO.Service.Common;
using SMO.Service.PS.Models;
using SMO.Service.PS.Models.Report;
using SMO.Service.PS.Models.Report.CustomerContract;
using SMO.Service.PS.Models.Report.ProjectCostControl;
using SMO.Service.PS.Models.Report.ProjectDetailDataCost;
using SMO.Service.PS.Models.Report.ResourceReport;
using SMO.Service.PS.Models.Report.SummaryProject;
using SMO.Service.PS.Models.Report.VendorMonitoring;
using SMO.Service.PS.Models.Report.VendorVolume;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using static SMO.SAPINT.Functions.FunctionPS;

namespace SMO.Service.PS
{
    public class ProjectService : PSService<T_PS_PROJECT, ProjectRepo>
    {
        internal void Search()
        {
            try
            {
                var isFullView = ProfileUtilities.User.IS_IGNORE_USER || AuthorizeUtilities.CheckUserRight("R00");
                var repo = (ProjectRepo)this.CurrentRepository;
                int iTotalRecord = 0;
                this.ObjList = repo.Search(this.ObjDetail, this.NumerRecordPerPage, this.Page, out iTotalRecord, ProfileUtilities.User.USER_NAME, isFullView).ToList();
                this.TotalRecord = iTotalRecord;
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
        internal void PostDataToSAP(Guid id)
        {
            this.Get(id);
            var lstProjectStruct = UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(
                        x => x.PROJECT_ID == this.ObjDetail.ID
                    ).OrderBy(x => x.C_ORDER).ToList();
            foreach (var item in lstProjectStruct)
            {
                item.QUANTITY = item.QUANTITY / 1.000000000000000000000000000000000m;
                item.TOTAL = item.TOTAL / 1.000000000000000000000000000000000m;
            }

            var lstCode = new List<string>() { "C.0397", "C.0390", "C.0382", "C.0383", "C.0387", "C.0380", "C.0389", "C.0395", "C.0384", "C.0381" };
            if (lstCode.Contains(this.ObjDetail.CODE))
            {
                return;
            }

            if (!this.ObjDetail.IS_CREATE_ON_SAP)
            {
                this.CreateProjectSAP(lstProjectStruct);
            }
            else
            {
                UpdateProjectSAP("A", lstProjectStruct);
                if (!this.State)
                {
                    return;
                }

                UpdateProjectSAP("U", lstProjectStruct);
                if (!this.State)
                {
                    return;
                }

                UpdateProjectSAP("D", lstProjectStruct);
            }
        }

        internal IEnumerable<VendorVolumeReportData> GenerateVendorVolumeReport(VendorVolumeReportModel model)
        {
            int total;
            var isFullView = ProfileUtilities.User.IS_IGNORE_USER || AuthorizeUtilities.CheckUserRight("R00");
            var currentUser = ProfileUtilities.User.USER_NAME;
            var projects = UnitOfWork.Repository<ProjectRepo>().Search(new T_PS_PROJECT
            {
                ID = model.ProjectId,
                DON_VI = model.CompanyId,
                PHONG_BAN = model.DepartmentId,
                STATUS = model.Status?.GetValue()
            }, int.MaxValue, 0, out total, currentUser, isFullView);
            var projectIds = projects.Select(x => x.ID);
            var contracts = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().GetContractVendors(projectIds, model.Vendor);
            var projectStructs = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => projectIds.Contains(x.PROJECT_ID))
                .OrderBy(x => x.PROJECT_ID).ThenBy(x => x.C_ORDER).ToList();
            var allUnits = UnitOfWork.Repository<Repository.Implement.MD.UnitRepo>().GetAll();

            var projectPeriods = GetProjectPeriods(projectIds);
            var inTimePeriodIds = GetAllInPeriodTimes(model, projectIds).Select(x => x.ID);
            var beforeTimePeriodIds = GetAllBeforePeriodTimes(model, projectIds).Select(x => x.ID);

            var planCost = UnitOfWork.Repository<PlanCostRepo>()
                .Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID))
                .ToList();
            var inPeriodPlanCostData = planCost.Where(x => inTimePeriodIds.Contains(x.TIME_PERIOD_ID));
            var beforePlanCostData = planCost.Where(x => beforeTimePeriodIds.Contains(x.TIME_PERIOD_ID));

            var allWorkVolumeHeaders = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID) && x.TO_DATE <= model.ToDate
                && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                .ToList();
            var allWorkVolumeHeaderIds = allWorkVolumeHeaders.Select(x => x.ID);
            var allWorkVolumeData = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                .Where(x => allWorkVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();
            var inPeriodVolumeData = allWorkVolumeData.Where(x => allWorkVolumeHeaders.Where(y => CheckInTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            var beforePeriodVolumeData = allWorkVolumeData.Where(x => allWorkVolumeHeaders.Where(y => CheckBeforeStartTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));

            var allAcceptVolumeHeaders = UnitOfWork.Repository<VolumeAcceptRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID) && x.TO_DATE <= model.ToDate
                && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                .ToList();
            var allAcceptVolumeHeaderIds = allAcceptVolumeHeaders.Select(x => x.ID);
            var allAcceptVolumeData = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
                .Where(x => allAcceptVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();
            var inPeriodAcceptData = allAcceptVolumeData.Where(x => allAcceptVolumeHeaders.Where(y => CheckInTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            var beforePeriodAcceptData = allAcceptVolumeData.Where(x => allAcceptVolumeHeaders.Where(y => CheckBeforeStartTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            return VendorVolumeData(projectIds,
                                    contracts,
                                    projectStructs,
                                    allUnits,
                                    inPeriodPlanCostData,
                                    beforePlanCostData,
                                    inPeriodAcceptData,
                                    beforePeriodAcceptData,
                                    inPeriodVolumeData,
                                    beforePeriodVolumeData).OrderBy(x => x.ProjectId).ThenBy(x => x.ContractCode).ThenBy(x => x.StructureOrder);
        }
        #region Header Text
        internal string GetCompanyTextExportExcel(string companyId)
        {
            if (string.IsNullOrEmpty(companyId))
            {
                return "Công ty:";
            }
            else
            {
                var company = UnitOfWork.Repository<OrganizeRepo>().Get(companyId);
                return $"Công ty: {company?.NAME}";
            }
        }
        internal string GetDepartmentTextExportExcel(string departmentId)
        {
            if (string.IsNullOrEmpty(departmentId))
            {
                return "Phòng ban phụ trách:";
            }
            else
            {
                var department = UnitOfWork.Repository<OrganizeRepo>().Get(departmentId);
                return $"Phòng ban phụ trách: {department?.NAME}";
            }
        }
        internal string GetVendorTextExportExcel(string vendorId)
        {
            if (string.IsNullOrEmpty(vendorId))
            {
                return "Nhà thầu:";
            }
            else
            {
                var vendor = UnitOfWork.Repository<Repository.Implement.MD.VendorRepo>().Get(vendorId);
                return $"Nhà thầu: {vendor?.NAME}";
            }
        }
        internal string GetCustomerTextExportExcel(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                return "Khách hàng:";
            }
            else
            {
                var customer = UnitOfWork.Repository<Repository.Implement.MD.CustomerRepo>().Get(customerId);
                return $"Khách hàng: {customer?.NAME}";
            }
        }
        internal string GetProjectTypeTextExportExcel(string projectType)
        {
            if (string.IsNullOrEmpty(projectType))
            {
                return "Loại dự án:";
            }
            else
            {
                var customer = UnitOfWork.Repository<Repository.Implement.MD.ProjectTypeRepo>().Get(projectType);
                return $"Loại dự án: {customer?.NAME}";
            }
        }
        internal string GetProjectLevelTextExportExcel(string projectLevel)
        {
            if (string.IsNullOrEmpty(projectLevel))
            {
                return "Cấp dự án:";
            }
            else
            {
                var customer = UnitOfWork.Repository<Repository.Implement.MD.ProjectLevelRepo>().Get(projectLevel);
                return $"Cấp dự án: {customer?.NAME}";
            }
        }
        internal string GetLeaderTextExportExcel(string leaderUserName)
        {
            if (string.IsNullOrEmpty(leaderUserName))
            {
                return "Lãnh đạo phụ trách:";
            }
            else
            {
                var leader = UnitOfWork.Repository<UserRepo>().Get(leaderUserName);
                return $"Lãnh đạo phụ trách: {leader?.FULL_NAME}";
            }
        }
        internal string GetStatusTextExportExcel(ProjectStatus? status)
        {
            if (!status.HasValue)
            {
                return "Trạng thái:";
            }
            else
            {
                return $"Trạng thái: {status.Value.GetName()}";
            }
        }
        internal string GetProjectTextExportExcel(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return "Dự án:";
            }
            else
            {
                var project = GetProject(projectId);
                return $"Dự án: {project?.NAME}";
            }
        }

        internal string GetContractTextExportExcel(Guid? contractId)
        {
            if (!contractId.HasValue)
            {
                return "Số Hợp đồng/Phụ lục:";
            }
            else
            {
                var contract = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Get(contractId.Value);
                return $"Số Hợp đồng/Phụ lục: {contract?.CONTRACT_NUMBER}";
            }
        }
       

        #endregion

        private IEnumerable<VendorVolumeReportData> VendorVolumeData(IEnumerable<Guid> projectIds,
                                                                     IList<T_PS_CONTRACT> contracts,
                                                                     IList<T_PS_PROJECT_STRUCT> projectStructs,
                                                                     IList<T_MD_UNIT> allUnits,
                                                                     IEnumerable<T_PS_PLAN_COST> inPeriodPlanCostData,
                                                                    IEnumerable<T_PS_PLAN_COST> beforePlanCostData,
                                                                     IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> inPeriodAcceptData,
                                                                     IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> beforePeriodAcceptData,
                                                                     IEnumerable<T_PS_VOLUME_WORK_DETAIL> inPeriodVolumeData,
                                                                     IEnumerable<T_PS_VOLUME_WORK_DETAIL> beforePeriodVolumeData
)
        {
            var inPeriodPlanVolume = 0M;
            var startPeriodWorkVolume = 0M;
            var startPeriodAcceptedVolume = 0M;
            var inPeriodWorkVolume = 0M;
            var inPeriodAcceptVolume = 0M;
            var endPeriodPlanCost = 0M;

            var dictInPeriodPlanVolume = inPeriodPlanCostData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictStructure = projectStructs.ToDictionary(x => x.ID, x => x);

            var dictBeforePeriodVolumeData = beforePeriodVolumeData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictBeforePeriodAcceptedVolume = beforePeriodAcceptData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictInPeriodWorkVolume = inPeriodVolumeData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictInPeriodAcceptData = inPeriodAcceptData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictBeforePlanCostData = beforePlanCostData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));

            var dictStructures = projectStructs.ToDictionary(x => x.ID, x => x);
            foreach (var projectId in projectIds)
            {
                var projectContracts = contracts.Where(x => x.PROJECT_ID == projectId);
                foreach (var contract in projectContracts)
                {
                    foreach (var contractDetail in contract.Details)
                    {
                        var structureId = contractDetail.PROJECT_STRUCT_ID;
                        var projectStruct = dictStructures[structureId];
                        var unit = allUnits.FirstOrDefault(x => x.CODE == contractDetail.UNIT_CODE);

                        var canGetInPeriodPlanVolume = dictInPeriodPlanVolume.TryGetValue(structureId, out inPeriodPlanVolume);
                        var canGetstartPeriodWorkVolume = dictBeforePeriodVolumeData.TryGetValue(structureId, out startPeriodWorkVolume);
                        var canGetStartPeriodAcceptedVolume = dictBeforePeriodAcceptedVolume.TryGetValue(structureId, out startPeriodAcceptedVolume);
                        var canGetInPeriodWorkVolume = dictInPeriodWorkVolume.TryGetValue(structureId, out inPeriodWorkVolume);
                        var canGetInPeriodAcceptedVolume = dictInPeriodAcceptData.TryGetValue(structureId, out inPeriodAcceptVolume);
                        var canGetEndPeriodPlanCost = dictBeforePlanCostData.TryGetValue(structureId, out endPeriodPlanCost);
                        yield return new VendorVolumeReportData
                        {
                            Id = projectStruct.ID,
                            ProjectId = projectId,
                            Type = projectStruct.TYPE,
                            StructureCode = projectStruct.GEN_CODE,
                            StructureOrder = projectStruct.C_ORDER,
                            ContractCode = contract.CONTRACT_NUMBER,
                            StructureName = projectStruct.TEXT,
                            VendorName = contract.Vendor.NAME,
                            VendorCode = contract.VENDOR_CODE,
                            UnitName = unit?.NAME,
                            UnitCode = projectStruct.UNIT_CODE,
                            TotalContractValue = projectStruct.QUANTITY,

                            StartPeriodWorkVolume = canGetstartPeriodWorkVolume ? startPeriodWorkVolume : 0,
                            StartPeriodAcceptVolume = canGetStartPeriodAcceptedVolume ? startPeriodAcceptedVolume : 0,
                            InPeriodPlanVolume = canGetInPeriodPlanVolume ? inPeriodPlanVolume : 0,
                            InPeriodWorkVolume = canGetInPeriodWorkVolume ? inPeriodWorkVolume : 0,
                            InPeriodAcceptVolume = canGetInPeriodAcceptedVolume ? inPeriodAcceptVolume : 0,
                            EndPeriodPlanCost = canGetEndPeriodPlanCost ? endPeriodPlanCost : 0
                        };
                    }
                }
            }

            yield break;
        }

        internal IEnumerable<CustomerContractReportData> GenerateCustomerContractReport(CustomerContractReportModel model)
        {
            int total;
            var isFullView = ProfileUtilities.User.IS_IGNORE_USER || AuthorizeUtilities.CheckUserRight("R00");
            var currentUser = ProfileUtilities.User.USER_NAME;
            var projects = UnitOfWork.Repository<ProjectRepo>().Search(new T_PS_PROJECT
            {
                ID = model.ProjectId,
                DON_VI = model.CompanyId,
                PHONG_BAN = model.DepartmentId,
                PROJECT_LEVEL_CODE = model.ProjectLevel,
                TYPE = model.ProjectType,
                GIAM_DOC_DU_AN = model.GiamDocDuAn,
                STATUS = model.Status?.GetValue()
            }, int.MaxValue, 0, out total, currentUser, isFullView);
            var projectIds = projects.Select(x => x.ID);
            var contractCustomers = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().GetContractCustomers(projectIds, model.Customer);
            var structureIds = contractCustomers.SelectMany(x => x.Details.Select(y => y.PROJECT_STRUCT_ID));
            var projectStructs = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID))
                .OrderBy(x => x.PROJECT_ID).ThenBy(x => x.C_ORDER)
                .ToList()
                .Where(x => structureIds.Contains(x.ID));
            var units = UnitOfWork.Repository<Repository.Implement.MD.UnitRepo>()
                .GetAll();
            var inTimePeriodIds = GetAllInPeriodTimes(model, projectIds).Select(x => x.ID);
            var beforeTimePeriodIds = GetAllBeforePeriodTimes(model, projectIds).Select(x => x.ID);

            var planCostData = UnitOfWork.Repository<PlanCostRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID))
                .ToList()
                .Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID) && (beforeTimePeriodIds.Contains(x.TIME_PERIOD_ID) || inTimePeriodIds.Contains(x.TIME_PERIOD_ID)));
            var inPeriodPlanCostData = planCostData.Where(x => inTimePeriodIds.Contains(x.TIME_PERIOD_ID));
            var beforePlanCostData = planCostData.Where(x => beforeTimePeriodIds.Contains(x.TIME_PERIOD_ID));

            var allWorkVolumeHeaders = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID) && x.TO_DATE <= model.ToDate
                && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                .ToList();
            var allWorkVolumeHeaderIds = allWorkVolumeHeaders.Select(x => x.ID);
            var allWorkVolumeData = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                .Where(x => allWorkVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();
            var inPeriodVolumeData = allWorkVolumeData.Where(x => allWorkVolumeHeaders.Where(y => CheckInTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            var beforePeriodVolumeData = allWorkVolumeData.Where(x => allWorkVolumeHeaders.Where(y => CheckBeforeStartTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));

            var allAcceptVolumeHeaders = UnitOfWork.Repository<VolumeAcceptRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID) && x.TO_DATE <= model.ToDate
                && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                .ToList();
            var allAcceptVolumeHeaderIds = allAcceptVolumeHeaders.Select(x => x.ID);
            var allAcceptVolumeData = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
                .Where(x => allAcceptVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();
            var inPeriodAcceptData = allAcceptVolumeData.Where(x => allAcceptVolumeHeaders.Where(y => CheckInTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            var beforePeriodAcceptData = allAcceptVolumeData.Where(x => allAcceptVolumeHeaders.Where(y => CheckBeforeStartTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));


            var listItems = CustomerContractData(projectIds,
                                                 contractCustomers,
                                                 units,
                                                 beforePlanCostData,
                                                 inPeriodVolumeData,
                                                 beforePeriodVolumeData,
                                                 inPeriodAcceptData,
                                                 beforePeriodAcceptData,
                                                 inPeriodPlanCostData,
                                                 projectStructs).ToList()
                .OrderBy(x => x.ContractCode).ThenBy(x => x.Order).ToList();
            foreach (var contractCode in listItems.GroupBy(x => x.ContractCode).Select(x => x.Key))
            {
                var firstIndex = listItems.FindIndex(x => x.ContractCode == contractCode);
                var allItemContracts = listItems.Where(x => x.ContractCode == contractCode);
                var parentIds = allItemContracts.Select(x => x.ParentId);
                listItems.Insert(firstIndex, new CustomerContractReportData
                {
                    ContractCode = contractCode,
                    StructureName = "Tổng cộng",
                    IsSummary = true,
                    ParentId = null,
                    Id = null,
                });
            }

            return CalculateCustomerContractDescendants(listItems);
        }

        private static IEnumerable<CustomerContractReportData> CustomerContractData(IEnumerable<Guid> projectIds,
                                                                                    IList<T_PS_CONTRACT> contractCustomers,
                                                                                    IList<T_MD_UNIT> units,
                                                                                    IEnumerable<T_PS_PLAN_COST> beforePlanCostData,
                                                                                    IEnumerable<T_PS_VOLUME_WORK_DETAIL> inPeriodVolumeData,
                                                                                    IEnumerable<T_PS_VOLUME_WORK_DETAIL> beforePeriodVolumeData,
                                                                                    IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> inPeriodAcceptData,
                                                                                    IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> beforePeriodAcceptData,
                                                                                    IEnumerable<T_PS_PLAN_COST> inPeriodPlanCostData,
                                                                                    IEnumerable<T_PS_PROJECT_STRUCT> projectStructs)
        {
            var inPeriodPlanVolume = 0M;
            var startPeriodWorkVolume = 0M;
            var startPeriodAcceptedVolume = 0M;
            var inPeriodWorkVolume = 0M;
            var inPeriodAcceptedVolume = 0M;
            var endPeriodPlanCost = 0M;

            var dictInPeriodPlanCostData = inPeriodPlanCostData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictStructure = projectStructs.ToDictionary(x => x.ID, x => x);

            var dictBeforePeriodVolumeData = beforePeriodVolumeData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictStartPeriodAcceptedVolume = beforePeriodAcceptData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictInPeriodWorkVolume = inPeriodVolumeData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictInPeriodAcceptData = inPeriodAcceptData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            var dictBeforePlanCostData = beforePlanCostData.GroupBy(x => x.PROJECT_STRUCT_ID).ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            foreach (var projectId in projectIds)
            {
                var contractProjects = contractCustomers.Where(x => x.PROJECT_ID == projectId).OrderBy(x => x.PARENT_CODE);
                foreach (var contract in contractProjects)
                {
                    foreach (var contractDetail in contract.Details)
                    {
                        var structureId = contractDetail.PROJECT_STRUCT_ID;
                        var projectStruct = dictStructure[structureId];
                        var unit = units.FirstOrDefault(x => x.CODE == projectStruct.UNIT_CODE);
                        var canGetInPeriodPlanVolume = dictInPeriodPlanCostData.TryGetValue(structureId, out inPeriodPlanVolume);
                        var canGetStartPeriodWorkVolume = dictBeforePeriodVolumeData.TryGetValue(structureId, out startPeriodWorkVolume);
                        var canGetStartPeriodAcceptedVolume = dictStartPeriodAcceptedVolume.TryGetValue(structureId, out startPeriodAcceptedVolume);
                        var canGetInPeriodWorkVolume = dictInPeriodWorkVolume.TryGetValue(structureId, out inPeriodWorkVolume);
                        var canGetInPeriodAcceptedVolume = dictInPeriodAcceptData.TryGetValue(structureId, out inPeriodAcceptedVolume);
                        var canGetEndPeriodPlanCost = dictBeforePlanCostData.TryGetValue(structureId, out endPeriodPlanCost);
                        yield return new CustomerContractReportData
                        {
                            Order = projectStruct.C_ORDER,
                            StructureCode = projectStruct.GEN_CODE,
                            StructureName = projectStruct.TEXT,
                            UnitName = unit?.NAME,
                            UnitCode = projectStruct.UNIT_CODE,
                            Price = projectStruct.PRICE ?? 0,
                            PlanVolume = projectStruct.PLAN_VOLUME ?? 0,
                            PlanTotal = projectStruct.PLAN_VOLUME * projectStruct.PRICE,
                            ContractValue = projectStruct.QUANTITY ?? 0,
                            ContractTotal = projectStruct.TOTAL ?? 0,
                            ContractCode = contract.CONTRACT_NUMBER,

                            StartPeriodWorkVolume = canGetStartPeriodWorkVolume ? startPeriodWorkVolume : 0,
                            StartPeriodAcceptedVolume = canGetStartPeriodAcceptedVolume ? startPeriodAcceptedVolume : 0,
                            StartPeriodTotalWorkVolume = canGetStartPeriodWorkVolume ? startPeriodWorkVolume * projectStruct.PRICE : 0,
                            StartPeriodTotalAcceptedVolume = canGetStartPeriodAcceptedVolume ? startPeriodAcceptedVolume * projectStruct.PRICE : 0,

                            InPeriodWorkVolume = canGetInPeriodWorkVolume ? inPeriodWorkVolume : 0,
                            InPeriodAcceptedVolume = canGetInPeriodAcceptedVolume ? inPeriodAcceptedVolume : 0,
                            InPeriodPlanVolume = canGetInPeriodPlanVolume ? inPeriodPlanVolume : 0,
                            InPeriodTotalWorkVolume = canGetInPeriodWorkVolume ? inPeriodWorkVolume * projectStruct.PRICE : 0,
                            InPeriodTotalAcceptedVolume = canGetInPeriodAcceptedVolume ? inPeriodAcceptedVolume * projectStruct.PRICE : 0,
                            InPeriodTotalPlanVolume = canGetInPeriodPlanVolume ? inPeriodPlanVolume * projectStruct.PRICE : 0,

                            EndPeriodPlanCost = canGetEndPeriodPlanCost ? endPeriodPlanCost : 0,
                            EndPeriodTotalPlanCost = canGetEndPeriodPlanCost ? endPeriodPlanCost * projectStruct.PRICE : 0,

                            Id = projectStruct.ID,
                            ParentId = projectStruct.PARENT_ID,
                        };
                    }
                }
            }
        }

        internal IEnumerable<SummaryProjectReportData> GenerateSummaryProjectReport(SummaryProjectReportModel model)
        {
            int total;
            var isFullView = ProfileUtilities.User.IS_IGNORE_USER || AuthorizeUtilities.CheckUserRight("R00");
            var currentUser = ProfileUtilities.User.USER_NAME;
            var projects = UnitOfWork.Repository<ProjectRepo>().Search(new T_PS_PROJECT
            {
                ID = model.ProjectId,
                DON_VI = model.CompanyId,
                PHONG_BAN = model.DepartmentId,
                PROJECT_LEVEL_CODE = model.ProjectLevel,
                TYPE = model.ProjectType,
                GIAM_DOC_DU_AN = model.GiamDocDuAn,
                STATUS = model.Status?.GetValue(),
                CUSTOMER_CODE = model.Customer
            }, int.MaxValue, 0, out total, currentUser, isFullView);
            var projectIds = projects.Select(x => x.ID);
            var contracts = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().GetContractCustomers(projectIds, null).ToList();
            var constractVendors = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().GetContractVendors(projectIds, null);
            contracts.AddRange(constractVendors);
            var contractIds = contracts.Select(x => x.ID);
            var dtSls = UnitOfWork.Repository<ProjectSlDtRepo>().Queryable().Where(x => projectIds.Contains(x.PROJECT_ID)).ToList();
            var costValues = UnitOfWork.Repository<PlanCostRepo>()
                    .Queryable()
                    .Where(x => projectIds.Contains(x.PROJECT_ID))
                    .ToList();
            var structures = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => projectIds.Contains(x.PROJECT_ID)).ToList();
            var volumeWorks = UnitOfWork.Repository<VolumeWorkRepo>()
                   .GetWithDetails(projectIds, ProjectWorkVolumeStatus.PHE_DUYET.GetValue(), isCustomer: null);
            var volumeAccepts = UnitOfWork.Repository<VolumeAcceptRepo>()
                   .GetWithDetails(projectIds, ProjectWorkVolumeStatus.PHE_DUYET.GetValue(), isCustomer: null);
            var contractPayments = UnitOfWork.Repository<ContractPaymentRepo>().Queryable()
                .Where(x => contractIds.Contains(x.CONTRACT_ID))
                .ToList();
            var projectTypes = UnitOfWork.Repository<Repository.Implement.MD.ProjectTypeRepo>().GetAll();
            var projectLevels = UnitOfWork.Repository<Repository.Implement.MD.ProjectLevelRepo>().GetAll();

            object lockIndex = new object();
            var tasks = new Task<SummaryProjectReportData>[projects.Count];
            var inTimePeriodIds = GetAllInPeriodTimes(model, projectIds).Select(x => x.ID).ToList();
            var beforeTimePeriodIds = GetAllBeforePeriodTimes(model, projectIds).Select(x => x.ID).ToList();
            var inYearTimePeriodIds = GetInYearPeriodTimes(model, projectIds).Select(x => x.ID).ToList();
            var users = UnitOfWork.Repository<UserRepo>().GetAll();
            var lstOrg = UnitOfWork.Repository<OrganizeRepo>().GetAll();

            var customerProjectCodes = projects.Select(y => y.CUSTOMER_CODE);
            var customerProjects = UnitOfWork.Repository<Repository.Implement.MD.CustomerRepo>().Queryable()
                .Where(x => customerProjectCodes.Contains(x.CODE))
                .ToList();
            var index = 0;
            foreach (var project in projects)
            {
                model.ProjectId = project.ID;
                var projectStructs = structures.Where(x => x.PROJECT_ID == project.ID).ToList();

                var projectDtSls = dtSls.Where(x => x.PROJECT_ID == project.ID);
                var customerContracts = contracts.Where(x => x.IS_SIGN_WITH_CUSTOMER && x.PROJECT_ID == project.ID);
                var vendorContracts = contracts.Where(x => !x.IS_SIGN_WITH_CUSTOMER && x.PROJECT_ID == project.ID);
                var customerContractIds = customerContracts.Select(y => y.ID);
                var vendorContractIds = vendorContracts.Select(y => y.ID);
                var customerContract = contracts.FirstOrDefault(x => x.IS_SIGN_WITH_CUSTOMER && x.PARENT_CODE == null && x.PROJECT_ID == project.ID);
                var projectContractCustomerPayments = contractPayments.Where(x => customerContractIds.Contains(x.CONTRACT_ID));
                var projectContractVendorPayments = contractPayments.Where(x => vendorContractIds.Contains(x.CONTRACT_ID));
                var vendorContractDetails = vendorContracts.SelectMany(x => x.Details).Distinct();

                var customer = customerProjects.FirstOrDefault(x => x.CODE == project.CUSTOMER_CODE);
                var inPeriodPlanCost = costValues.Where(x => inTimePeriodIds.Contains(x.TIME_PERIOD_ID) && x.PROJECT_ID == project.ID).ToList();
                var beforePeriodPlanCost = costValues.Where(x => beforeTimePeriodIds.Contains(x.TIME_PERIOD_ID) && x.PROJECT_ID == project.ID).ToList();
                var inPeriodProjectWolumeWorkBoqs = volumeWorks.Where(x => x.PROJECT_ID == project.ID && CheckInTimePeriod(x.TO_DATE.Value, model) && x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var inYearProjectWolumeWorkBoqs = volumeWorks.Where(x => x.PROJECT_ID == project.ID && CheckDuringYearTimePeriod(x.TO_DATE.Value, model) && x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var beforePeriodProjectWolumeWorkBoqs = volumeWorks.Where(x => x.PROJECT_ID == project.ID && CheckBeforeTimePeriod(x.TO_DATE.Value, model) && x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var inYearProjectWolumeWorkCosts = volumeWorks.Where(x => x.PROJECT_ID == project.ID && CheckDuringYearTimePeriod(x.TO_DATE.Value, model) && !x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var inPeriodProjectWolumeWorkCosts = volumeWorks.Where(x => x.PROJECT_ID == project.ID && CheckInTimePeriod(x.TO_DATE.Value, model) && !x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var beforePeriodProjectWolumeWorkCosts = volumeWorks.Where(x => x.PROJECT_ID == project.ID && CheckBeforeTimePeriod(x.TO_DATE.Value, model) && !x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();

                var inPeriodProjectWolumeAcceptBoqs = volumeAccepts.Where(x => x.PROJECT_ID == project.ID && CheckInTimePeriod(x.TO_DATE.Value, model) && x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var inYearProjectWolumeAcceptBoqs = volumeAccepts.Where(x => x.PROJECT_ID == project.ID && CheckDuringYearTimePeriod(x.TO_DATE.Value, model) && x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var beforePeriodProjectWolumeAcceptBoqs = volumeAccepts.Where(x => x.PROJECT_ID == project.ID && CheckBeforeTimePeriod(x.TO_DATE.Value, model) && x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var inPeriodProjectWolumeAcceptCosts = volumeAccepts.Where(x => x.PROJECT_ID == project.ID && CheckInTimePeriod(x.TO_DATE.Value, model) && !x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var inYearProjectWolumeAcceptCosts = volumeAccepts.Where(x => x.PROJECT_ID == project.ID && CheckDuringYearTimePeriod(x.TO_DATE.Value, model) && !x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();
                var beforePeriodProjectWolumeAcceptCosts = volumeAccepts.Where(x => x.PROJECT_ID == project.ID && CheckBeforeTimePeriod(x.TO_DATE.Value, model) && !x.IS_CUSTOMER)
                    .SelectMany(x => x.Details)
                    .Distinct();

                var projectContracts = contracts.Where(x => x.PROJECT_ID == project.ID);


                yield return new SummaryProjectReportData
                {
                    ProjectName = $"{project.CODE} - {project.NAME}",
                    Customer = $"{customer?.CODE} - {customer?.NAME}",
                    SignedDate = customerContract?.START_DATE.Value.ToString(Global.DateToStringFormat),
                    ContractCode = customerContract?.CONTRACT_NUMBER,
                    ContractTotal = customerContracts.Sum(x => x.CONTRACT_VALUE),
                    InPeriodPlanValue = CalculatePlanCost(inPeriodPlanCost, projectStructs, isCustomer: true),
                    InPeriodPerformanceValue = inPeriodProjectWolumeWorkBoqs.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    InPeriodPlannedRevenue = projectDtSls.Where(x => inTimePeriodIds.Contains(x.TIME_PERIOD_ID)
                        && x.CRITERIA_CODE == ProjectCriteria.DOANH_THU.GetValue()).Sum(x => x.VALUE),
                    InPeriodPerformanceRevenue = inPeriodProjectWolumeAcceptBoqs.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    InPeriodPlanCosts = CalculatePlanCost(inPeriodPlanCost, projectStructs, isCustomer: false),
                    InPeriodVolumeWorkCosts = inPeriodProjectWolumeWorkCosts.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    InPeriodImplementedCosts = inPeriodProjectWolumeAcceptCosts.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    InPeriodPlannedProceeds = projectDtSls.Where(x => inTimePeriodIds.Contains(x.TIME_PERIOD_ID)
                        && x.CRITERIA_CODE == ProjectCriteria.TIEN_THU.GetValue()).Sum(x => x.VALUE),
                    InPeriodProceedsMadeMoney = projectContractCustomerPayments.Where(x => CheckInTimePeriod(x.PAYMENT_DATE, model))
                        .Sum(x => x.AMOUNT_ADVANCE + x.AMOUNT),
                    InPeriodPlanSpentMoney = projectDtSls.Where(x => inTimePeriodIds.Contains(x.TIME_PERIOD_ID)
                        && x.CRITERIA_CODE == ProjectCriteria.TIEN_CHI.GetValue()).Sum(x => x.VALUE),
                    InPeriodImplementedSpentMoney = projectContractVendorPayments.Where(x => CheckInTimePeriod(x.PAYMENT_DATE, model))
                        .Sum(x => x.AMOUNT_ADVANCE + x.AMOUNT),
                    DuringYearPerformanceValue = inYearProjectWolumeWorkBoqs.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    DuringYearPerformanceRevenue = inYearProjectWolumeAcceptBoqs.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    DuringYearVolumeWorkCosts = inYearProjectWolumeWorkCosts.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    DuringYearImplementedCosts = inYearProjectWolumeAcceptCosts.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    DuringYearProceedsMadeMoney = projectContractCustomerPayments.Where(x => CheckDuringYearTimePeriod(x.PAYMENT_DATE, model))
                        .Sum(x => x.AMOUNT_ADVANCE + x.AMOUNT),
                    DuringYearImplementedSpentMoney = projectContractVendorPayments.Where(x => CheckDuringYearTimePeriod(x.PAYMENT_DATE, model))
                        .Sum(x => x.AMOUNT_ADVANCE + x.AMOUNT),
                    AccumulatedPerformanceValue = beforePeriodProjectWolumeWorkBoqs.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    AccumulatedPerformanceRevenue = beforePeriodProjectWolumeAcceptBoqs.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    AccumulatedImplementedCosts = beforePeriodProjectWolumeAcceptCosts.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    AccumulatedVolumeWorkCosts = beforePeriodProjectWolumeWorkCosts.Sum(x => Math.Round(x.VALUE * x.PRICE, 0)),
                    AccumulatedProceedsMadeMoney = projectContractCustomerPayments.Where(x => CheckBeforeTimePeriod(x.PAYMENT_DATE, model))
                        .Sum(x => x.AMOUNT_ADVANCE + x.AMOUNT),
                    AccumulatedImplementedSpentMoney = projectContractVendorPayments.Where(x => CheckBeforeTimePeriod(x.PAYMENT_DATE, model))
                        .Sum(x => x.AMOUNT_ADVANCE + x.AMOUNT),
                    AccumulatedPlannedRevenue = projectDtSls.Where(x => beforeTimePeriodIds.Contains(x.TIME_PERIOD_ID)
                        && x.CRITERIA_CODE == ProjectCriteria.DOANH_THU.GetValue()).Sum(x => x.VALUE),
                    AccumulatedPlannedValue = CalculatePlanCost(beforePeriodPlanCost, projectStructs, isCustomer: true),
                    AccumulatedPlanCost = CalculatePlanCost(beforePeriodPlanCost, projectStructs, isCustomer: false),
                    Status = project.STATUS.GetEnum<ProjectStatus>().GetName(),
                    Type = projectTypes.FirstOrDefault(x => x.CODE == project.TYPE)?.NAME,
                    TotalCost = (decimal?)project?.TOTAL_COST,
                    ProjectManager = users.FirstOrDefault(x => x.USER_NAME == project.QUAN_TRI_DU_AN)?.FULL_NAME,
                    DonVi = lstOrg.FirstOrDefault(x => x.PKID == project.DON_VI)?.NAME,
                    GiamDocDuAn = users.FirstOrDefault(x => x.USER_NAME == project.GIAM_DOC_DU_AN)?.FULL_NAME,
                    Level = projectLevels.FirstOrDefault(x => x.CODE == project.PROJECT_LEVEL_CODE)?.NAME,
                    PhongBan = lstOrg.FirstOrDefault(x => x.PKID == project.PHONG_BAN)?.NAME,
                    Index = ++index
                };
            }
        }

        internal List<ProjectCostControlReportData> GenerateProjectCostControlReport(ProjectCostControlReportModel model)
        {
            int total;
            var isFullView = ProfileUtilities.User.IS_IGNORE_USER || AuthorizeUtilities.CheckUserRight("R00");
            var currentUser = ProfileUtilities.User.USER_NAME;
            var projects = UnitOfWork.Repository<ProjectRepo>().Search(new T_PS_PROJECT
            {
                ID = model.ProjectId,
                DON_VI = model.CompanyId,
                PHONG_BAN = model.DepartmentId,
                PROJECT_LEVEL_CODE = model.ProjectLevel,
                TYPE = model.ProjectType,
                GIAM_DOC_DU_AN = model.GiamDocDuAn,
                STATUS = model.Status?.GetValue()
            }, int.MaxValue, 0, out total, currentUser, isFullView);

            var dataCus = new List<ProjectCostControlReportData>();
            var dataVen = new List<ProjectCostControlReportData>();
            var data = new List<ProjectCostControlReportData>();

            var order = 1;

            foreach (var project in projects.OrderBy(x => x.CODE))
            {
                var projectStructCustomer = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID && x.TYPE == "BOQ");
                var projectStructVendor = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID && x.TYPE != "BOQ");

                var contractCustomer = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID && x.CONTRACT_TYPE.Contains("KD")).FirstOrDefault();
                if (contractCustomer != null)
                {
                    var contractCustomerDetail = UnitOfWork.Repository<ContractDetailRepo>().Queryable().Where(x => x.CONTRACT_ID == contractCustomer.ID).ToList();
                }

                var contractVendor = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID && !x.CONTRACT_TYPE.Contains("KD")).ToList();
                var contractVendorDetail = (from x in contractVendor
                                            join y in UnitOfWork.Repository<ContractDetailRepo>().GetAll() on x.ID equals y.CONTRACT_ID
                                            select new
                                            {
                                                detail = y
                                            }).ToList();

                var volumeWorkCustomer = UnitOfWork.Repository<VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID && x.IS_CUSTOMER == true && x.STATUS == "05" && model.FromDate <= x.TO_DATE && model.ToDate >= x.TO_DATE).ToList();
                var volumeWorkCustomerDetail = (from x in volumeWorkCustomer
                                                join y in UnitOfWork.Repository<VolumeWorkDetailRepo>().GetAll() on x.ID equals y.HEADER_ID
                                                let vendor = x.Vendor?.NAME
                                                group y by y.PROJECT_STRUCT_ID into a
                                                select new
                                                {
                                                    projectStructId = a.Key,
                                                    value = a.Sum(b => b.VALUE),
                                                    total = a.Sum(b => b.TOTAL),
                                                }).ToList();
                var volumeWorkVendor = UnitOfWork.Repository<VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID && x.IS_CUSTOMER == false && x.STATUS == "05" && model.FromDate <= x.TO_DATE && model.ToDate >= x.TO_DATE).ToList();
                var volumeWorkVendorDetail = (from x in volumeWorkVendor
                                              join y in UnitOfWork.Repository<VolumeWorkDetailRepo>().GetAll() on x.ID equals y.HEADER_ID
                                              group y by y.PROJECT_STRUCT_ID into a
                                              select new
                                              {
                                                  projectStructId = a.Key,
                                                  value = a.Sum(b => b.VALUE),
                                                  price = a.First().PRICE,
                                                  total = a.Sum(b => b.TOTAL),
                                              }).ToList();

                var activityReferenceBoq = (from x in UnitOfWork.Repository<ActivityRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID)
                                            select new
                                            {
                                                Id = x.ID,
                                                boqReference = x.BOQ_REFRENCE_ID
                                            }).ToList();


                var wbsReferenceBoq = (from x in UnitOfWork.Repository<WbsRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID)
                                       select new
                                       {
                                           Id = x.ID,
                                           boqReference = x.BOQ_REFRENCE_ID
                                       }).ToList();
                var boqReference = activityReferenceBoq.Union(wbsReferenceBoq);
                var hasBoqRefrence = boqReference.Where(x => x.boqReference != null).ToList();
                if (hasBoqRefrence.Count() > 0)
                {
                    foreach (var cus in projectStructCustomer.OrderBy(c => c.C_ORDER))
                    {
                        var workCustomer = volumeWorkCustomerDetail.Where(c => c.projectStructId == cus.ID);
                        var rangeCus = new ProjectCostControlReportData
                        {
                            Id = cus.ID,
                            ParentId = cus.PARENT_ID,
                            Type = cus.TYPE,
                            ProjectCode = project.CODE,
                            StructureCode = cus.GEN_CODE,
                            StructureName = cus.TEXT,
                            UnitCode = cus.UNIT_CODE,
                            UnitName = cus.UNIT_CODE,
                            MainContractVolume = cus.QUANTITY,
                            MainContractPrice = cus.PRICE,
                            MainContractTotal = cus.TOTAL,
                            PerformanceContractVolume = workCustomer.Sum(c => c.value),
                            PerformanceContractTotal = workCustomer.Sum(c => c.total)
                        };
                        dataCus.Add(rangeCus);
                    }
                    foreach (var ven in projectStructVendor.OrderBy(v => v.C_ORDER))
                    {
                        var workVendor = volumeWorkVendorDetail.Where(v => v.projectStructId == ven.ID);
                        var contractDetail = contractVendorDetail.FirstOrDefault(x => x.detail.PROJECT_STRUCT_ID == ven.ID);
                        var contract = contractVendor.FirstOrDefault(x => x.ID == contractDetail?.detail?.CONTRACT_ID);
                        var rangeVen = new ProjectCostControlReportData
                        {
                            Id = ven.ID,
                            ParentId = ven.PARENT_ID,
                            LinkedBoqId = ven.ID,
                            Type = ven.TYPE,
                            ActivityId = ven.Activity?.ID,
                            WbsId = ven.Wbs?.ID,
                            VendorName = contract?.Vendor?.NAME,
                            ProjectCode = project.CODE,
                            StructureCode = ven.GEN_CODE,
                            StructureName = ven.TEXT,
                            UnitCode = ven.UNIT_CODE,
                            UnitName = ven.UNIT_CODE,
                            CostPlanVolume = ven.QUANTITY,
                            CostPlanPrice = ven.PRICE,
                            CostPlanTotal = ven.QUANTITY * ven.PRICE,
                            PerformanceCostVolume = workVendor.Sum(v => v.value),
                            PerformanceCostPrice = contractDetail?.detail?.UNIT_PRICE,
                            PerformanceCostTotal = workVendor.Sum(v => v.total)
                        };
                        dataVen.Add(rangeVen);
                    }

                    foreach (var ven in projectStructVendor.OrderBy(v => v.C_ORDER))
                    {
                        var checkErrorObj = true;
                        try
                        {
                            var act = ven.Activity?.BOQ_REFRENCE_ID;
                            var wbs = ven.Wbs?.BOQ_REFRENCE_ID;
                        }
                        catch (Exception ex)
                        {
                            checkErrorObj = false;
                        }
                        if (checkErrorObj == false)
                        {
                            var workVendor = volumeWorkVendorDetail.Where(v => v.projectStructId == ven.ID);
                                var contractDetail = contractVendorDetail.FirstOrDefault(x => x.detail.PROJECT_STRUCT_ID == ven.ID);
                                var contract = contractVendor.FirstOrDefault(x => x.ID == contractDetail?.detail?.CONTRACT_ID);
                                var rangeNoRef = new ProjectCostControlReportData
                                {
                                    Id = ven.ID,
                                    ParentId = ven.PARENT_ID,
                                    Type = ven.TYPE,
                                    Order = order++,
                                    OrderNullRef = ven.TYPE == "PROJECT" ? 0 : dataVen.Count() + dataCus.Count() + order++,
                                    VendorName = contract?.Vendor?.NAME,
                                    ProjectCode = project.CODE,
                                    StructureCode = ven.GEN_CODE,
                                    StructureName = ven.TEXT,
                                    UnitCode = ven.UNIT_CODE,
                                    UnitName = ven.UNIT_CODE,
                                    CostPlanVolume = ven.QUANTITY,
                                    CostPlanPrice = ven.PRICE,
                                    CostPlanTotal = ven.QUANTITY * ven.PRICE,
                                    PerformanceCostVolume = workVendor.Sum(v => v.value),
                                    PerformanceCostPrice = contractDetail?.detail?.UNIT_PRICE,
                                    PerformanceCostTotal = workVendor.Sum(v => v.total)
                                };
                                data.Add(rangeNoRef);
                        }
                        else
                        {
                            if (ven.Activity?.BOQ_REFRENCE_ID == null && ven.Wbs?.BOQ_REFRENCE_ID == null)
                            {
                                var workVendor = volumeWorkVendorDetail.Where(v => v.projectStructId == ven.ID);
                                var contractDetail = contractVendorDetail.FirstOrDefault(x => x.detail.PROJECT_STRUCT_ID == ven.ID);
                                var contract = contractVendor.FirstOrDefault(x => x.ID == contractDetail?.detail?.CONTRACT_ID);
                                var rangeNoRef = new ProjectCostControlReportData
                                {
                                    Id = ven.ID,
                                    ParentId = ven.PARENT_ID,
                                    Type = ven.TYPE,
                                    Order = order++,
                                    OrderNullRef = ven.TYPE == "PROJECT" ? 0 : dataVen.Count() + dataCus.Count() + order++,
                                    VendorName = contract?.Vendor?.NAME,
                                    ProjectCode = project.CODE,
                                    StructureCode = ven.GEN_CODE,
                                    StructureName = ven.TEXT,
                                    UnitCode = ven.UNIT_CODE,
                                    UnitName = ven.UNIT_CODE,
                                    CostPlanVolume = ven.QUANTITY,
                                    CostPlanPrice = ven.PRICE,
                                    CostPlanTotal = ven.QUANTITY * ven.PRICE,
                                    PerformanceCostVolume = workVendor.Sum(v => v.value),
                                    PerformanceCostPrice = contractDetail?.detail?.UNIT_PRICE,
                                    PerformanceCostTotal = workVendor.Sum(v => v.total)
                                };
                                data.Add(rangeNoRef);
                            }
                        }


                    }
                    foreach (var i in dataCus)
                    {
                        var referenceBoq = boqReference.Where(x => x.boqReference == i.Id);
                        var range = new ProjectCostControlReportData
                        {
                            Id = i.Id,
                            ParentId = i.ParentId,
                            Type = i.Type,
                            Order = order++,
                            OrderNullRef = 1,
                            ProjectCode = i.ProjectCode,
                            StructureCode = i.StructureCode,
                            StructureName = i.StructureName,
                            UnitCode = i.UnitCode,
                            UnitName = i.UnitName,
                            MainContractVolume = i.MainContractVolume,
                            MainContractPrice = i.MainContractPrice,
                            MainContractTotal = i.MainContractTotal,
                            PerformanceContractVolume = i.PerformanceContractVolume,
                            PerformanceContractTotal = i.PerformanceContractVolume * i.MainContractPrice
                        };
                        data.Add(range);
                        foreach (var refe in referenceBoq)
                        {
                            var ven = dataVen.Where(x => x.ActivityId == refe.Id || x.WbsId == refe.Id).ToList();
                            if (ven.FirstOrDefault() != null)
                            {
                                var rangeChild = new ProjectCostControlReportData
                                {
                                    Id = ven.FirstOrDefault().Id,
                                    ParentId = ven.FirstOrDefault().ParentId,
                                    LinkedBoqId = i.Id,
                                    Type = ven.FirstOrDefault().Type,
                                    Order = order++,
                                    OrderNullRef = 1,
                                    VendorName = ven.FirstOrDefault().VendorName,
                                    ProjectCode = ven.FirstOrDefault().ProjectCode,
                                    StructureCode = ven.FirstOrDefault().StructureCode,
                                    StructureName = ven.FirstOrDefault().StructureName,
                                    UnitCode = ven.FirstOrDefault().UnitCode,
                                    UnitName = ven.FirstOrDefault().UnitName,
                                    CostPlanVolume = ven.FirstOrDefault().CostPlanVolume,
                                    CostPlanPrice = ven.FirstOrDefault().CostPlanPrice,
                                    CostPlanTotal = ven.FirstOrDefault().CostPlanTotal,
                                    PerformanceCostVolume = ven.FirstOrDefault().PerformanceCostVolume,
                                    PerformanceCostPrice = ven.FirstOrDefault().PerformanceCostPrice,
                                    PerformanceCostTotal = ven.FirstOrDefault().PerformanceCostTotal,
                                };
                                data.Add(rangeChild);

                            }
                        }
                    }
                }
                else
                {
                    foreach (var cus in projectStructCustomer.OrderBy(c => c.C_ORDER))
                    {
                        var workCustomer = volumeWorkCustomerDetail.Where(c => c.projectStructId == cus.ID);
                        var rangeCus = new ProjectCostControlReportData
                        {
                            Id = cus.ID,
                            ParentId = cus.PARENT_ID,
                            Type = cus.TYPE,
                            Order = order++,
                            ProjectCode = project.CODE,
                            StructureCode = cus.GEN_CODE,
                            StructureName = cus.TEXT,
                            UnitCode = cus.UNIT_CODE,
                            UnitName = cus.UNIT_CODE,
                            MainContractVolume = cus.QUANTITY,
                            MainContractPrice = cus.PRICE,
                            MainContractTotal = cus.TOTAL,
                            PerformanceContractVolume = workCustomer.Sum(c => c.value),
                            PerformanceContractTotal = workCustomer.Sum(c => c.total)
                        };
                        data.Add(rangeCus);
                    }
                    foreach (var ven in projectStructVendor.OrderBy(v => v.C_ORDER))
                    {
                        var workVendor = volumeWorkVendorDetail.Where(v => v.projectStructId == ven.ID);
                        var contractDetail = contractVendorDetail.FirstOrDefault(x => x.detail.PROJECT_STRUCT_ID == ven.ID);
                        var contract = contractVendor.FirstOrDefault(x => x.ID == contractDetail?.detail?.CONTRACT_ID);
                        var rangeVen = new ProjectCostControlReportData
                        {
                            Id = ven.ID,
                            ParentId = ven.PARENT_ID,
                            LinkedBoqId = ven.ID,
                            Type = ven.TYPE,
                            Order = ven.TYPE == "PROJECT" ? 0 : order++,
                            VendorName = contract?.Vendor?.NAME,
                            ProjectCode = project.CODE,
                            StructureCode = ven.GEN_CODE,
                            StructureName = ven.TEXT,
                            UnitCode = ven.UNIT_CODE,
                            UnitName = ven.UNIT_CODE,
                            CostPlanVolume = ven.QUANTITY,
                            CostPlanPrice = ven.PRICE,
                            CostPlanTotal = ven.QUANTITY * ven.PRICE,
                            PerformanceCostVolume = workVendor.Sum(v => v.value),
                            PerformanceCostPrice = contractDetail?.detail?.UNIT_PRICE,
                            PerformanceCostTotal = workVendor.Sum(v => v.total)
                        };
                        data.Add(rangeVen);
                    }
                }

            }
            var descendantData = CalculateDescendants(data).ToList();
            data.AddRange(descendantData);
            return data;
        }

        private IEnumerable<ProjectCostControlReportData> CalculateDescendants(IList<ProjectCostControlReportData> data)
        {
            var lookup = data.ToLookup(i => i.ParentId);
            Queue<ProjectCostControlReportData> st = new Queue<ProjectCostControlReportData>(lookup[null]);
            while (st.Count > 0)
            {
                // get first item in queue
                var item = st.Dequeue();
                // variable to check should return item or not
                bool shouldReturn = true;
                // lst to store children of item which have children
                var lstHasChild = new List<ProjectCostControlReportData>();
                // loop through items which have parent id = item id
                foreach (var i in lookup[item.Id])
                {
                    if (lookup[i.Id].Count() > 0)
                    {
                        shouldReturn = false;
                        lstHasChild.Add(i);
                        st.Enqueue(i);
                    }
                    else
                    {
                        item.PerformanceCostTotal += i.PerformanceCostTotal ?? 0;
                        item.PerformanceContractTotal += i.PerformanceContractTotal ?? 0;
                        item.CostPlanTotal += i.CostPlanTotal ?? 0;
                        yield return i;
                    }
                }

                // remove all child of item
                // include children have child
                lookup = lookup
                    .Where(x => x.Key != item.Id)
                    .SelectMany(x => x)
                    .ToLookup(l => l.ParentId);
                if (shouldReturn)
                {
                    yield return item;
                }
                else
                {
                    // add children of item which have chilren to lookup 
                    if (lstHasChild.Count > 0)
                    {
                        lookup = lookup
                            .SelectMany(l => l)
                            .Concat(lstHasChild)
                            .ToLookup(x => x.ParentId);
                    }
                    // re-enqueue item to queue
                    st.Enqueue(item);
                }
            }
        }
        private IEnumerable<CustomerContractReportData> CalculateCustomerContractDescendants(IList<CustomerContractReportData> data)
        {
            var contractParentIds = data.GroupBy(x => x.ContractCode).ToDictionary(x => x.Key, x => x.Select(y => y.Id).Distinct());
            for (int i = data.Count - 1; i >= 0; i--)
            {
                var item = data[i];
                if (item.Id != null)
                {
                    var lst = data.Where(x => x.ContractCode == item.ContractCode && x.ParentId == item.Id);
                    item.InPeriodTotalPlanVolume = item.InPeriodTotalPlanVolume > 0 ?
                        item.InPeriodTotalPlanVolume :
                        lst.Sum(x => x.InPeriodTotalPlanVolume);
                    item.StartPeriodTotalWorkVolume = item.StartPeriodTotalWorkVolume > 0 ?
                        item.StartPeriodTotalWorkVolume : lst.Sum(x => x.StartPeriodTotalWorkVolume);
                    item.StartPeriodTotalAcceptedVolume = item.StartPeriodTotalAcceptedVolume > 0 ?
                        item.StartPeriodTotalAcceptedVolume : lst.Sum(x => x.StartPeriodTotalAcceptedVolume);
                    item.InPeriodTotalWorkVolume = item.InPeriodTotalWorkVolume > 0 ?
                            item.InPeriodTotalWorkVolume : lst.Sum(x => x.InPeriodTotalWorkVolume);
                    item.InPeriodTotalAcceptedVolume = item.InPeriodTotalAcceptedVolume > 0 ?
                            item.InPeriodTotalAcceptedVolume : lst.Sum(x => x.InPeriodTotalAcceptedVolume);
                    item.EndPeriodTotalPlanCost = item.EndPeriodTotalPlanCost > 0 ?
                            item.EndPeriodTotalPlanCost : lst.Sum(x => x.EndPeriodTotalPlanCost);
                    item.PlanTotal = item.PlanTotal > 0 ?
                            item.PlanTotal : lst.Sum(x => x.PlanTotal);
                    item.ContractTotal = item.ContractTotal > 0 ?
                            item.ContractTotal : lst.Sum(x => x.ContractTotal);

                }
                else
                {
                    var lst = data.Where(x => x.ContractCode == item.ContractCode
                        && !contractParentIds[item.ContractCode].Contains(x.ParentId));
                    item.InPeriodTotalPlanVolume = item.InPeriodTotalPlanVolume > 0 ?
                        item.InPeriodTotalPlanVolume : lst.Sum(x => x.InPeriodTotalPlanVolume);
                    item.StartPeriodTotalWorkVolume = item.StartPeriodTotalWorkVolume > 0 ?
                        item.StartPeriodTotalWorkVolume : lst.Sum(x => x.StartPeriodTotalWorkVolume);
                    item.StartPeriodTotalAcceptedVolume = item.StartPeriodTotalAcceptedVolume > 0 ?
                        item.StartPeriodTotalAcceptedVolume : lst.Sum(x => x.StartPeriodTotalAcceptedVolume);
                    item.InPeriodTotalWorkVolume = item.InPeriodTotalWorkVolume > 0 ?
                            item.InPeriodTotalWorkVolume : lst.Sum(x => x.InPeriodTotalWorkVolume);
                    item.InPeriodTotalAcceptedVolume = item.InPeriodTotalAcceptedVolume > 0 ?
                            item.InPeriodTotalAcceptedVolume : lst.Sum(x => x.InPeriodTotalAcceptedVolume);
                    item.EndPeriodTotalPlanCost = item.EndPeriodTotalPlanCost > 0 ?
                            item.EndPeriodTotalPlanCost : lst.Sum(x => x.EndPeriodTotalPlanCost);
                    item.PlanTotal = item.PlanTotal > 0 ?
                            item.PlanTotal : lst.Sum(x => x.PlanTotal);
                    item.ContractTotal = item.ContractTotal > 0 ?
                            item.ContractTotal : lst.Sum(x => x.ContractTotal);
                }
            }
            return data;
        }

        private decimal CalculatePlanCost(IList<T_PS_PLAN_COST> planCost, IList<T_PS_PROJECT_STRUCT> projectStructs, bool isCustomer)
        {
            var dictValue = planCost.Where(x => x.IS_CUSTOMER == isCustomer)
                .GroupBy(x => x.PROJECT_STRUCT_ID)
                .ToDictionary(x => x.Key, x => x.Sum(y => y.VALUE));
            return new ProjectSlDtService().CalculateTotal(projectStructs, dictValue);
        }

        internal IEnumerable<VendorMonitoringReportData> GenerateVendorMonitoringReport(VendorMonitoringReportModel model)
        {
            int total;
            var isFullView = ProfileUtilities.User.IS_IGNORE_USER || AuthorizeUtilities.CheckUserRight("R00");
            var currentUser = ProfileUtilities.User.USER_NAME;
            var projects = UnitOfWork.Repository<ProjectRepo>().Search(new T_PS_PROJECT
            {
                ID = model.ProjectId,
                DON_VI = model.CompanyId,
                PHONG_BAN = model.DepartmentId,
                PROJECT_LEVEL_CODE = model.ProjectLevel,
                TYPE = model.ProjectType,
                GIAM_DOC_DU_AN = model.GiamDocDuAn,
                STATUS = model.Status?.GetValue()
            }, int.MaxValue, 0, out total, currentUser, isFullView);
            var projectIds = projects.Select(x => x.ID);
            var contracts = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().GetContractVendors(projectIds, model.Vendor);
            var structureIds = contracts.SelectMany(x => x.Details.Select(y => y.PROJECT_STRUCT_ID));
            var projectStructs = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID))
                .OrderBy(x => x.PROJECT_ID).ThenBy(x => x.C_ORDER)
                .ToList()
                .Where(x => structureIds.Contains(x.ID));
            var units = UnitOfWork.Repository<Repository.Implement.MD.UnitRepo>().GetAll();
            var inTimePeriodIds = GetAllInPeriodTimes(model, projectIds).Select(x => x.ID);
            var beforeTimePeriodIds = GetAllBeforePeriodTimes(model, projectIds).Select(x => x.ID);
            var planCostData = UnitOfWork.Repository<PlanCostRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID))
                .ToList()
                .Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID) && (beforeTimePeriodIds.Contains(x.TIME_PERIOD_ID) || inTimePeriodIds.Contains(x.TIME_PERIOD_ID)));
            var inPeriodPlanCostData = planCostData.Where(x => inTimePeriodIds.Contains(x.TIME_PERIOD_ID));
            var beforePlanCostData = planCostData.Where(x => beforeTimePeriodIds.Contains(x.TIME_PERIOD_ID));

            var allWorkVolumeHeaders = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID) && x.TO_DATE <= model.ToDate
                && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                .ToList();
            var allWorkVolumeHeaderIds = allWorkVolumeHeaders.Select(x => x.ID);
            var allWorkVolumeData = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                .Where(x => allWorkVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();
            var inPeriodVolumeData = allWorkVolumeData.Where(x => allWorkVolumeHeaders.Where(y => CheckInTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            var beforePeriodVolumeData = allWorkVolumeData.Where(x => allWorkVolumeHeaders.Where(y => CheckBeforeTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));

            var allAcceptVolumeHeaders = UnitOfWork.Repository<VolumeAcceptRepo>().Queryable()
                .Where(x => projectIds.Contains(x.PROJECT_ID) && x.TO_DATE <= model.ToDate
                && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                .ToList();
            var allAcceptVolumeHeaderIds = allAcceptVolumeHeaders.Select(x => x.ID);
            var allAcceptVolumeData = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
                .Where(x => allAcceptVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();
            var inPeriodAcceptData = allAcceptVolumeData.Where(x => allAcceptVolumeHeaders.Where(y => CheckInTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            var beforePeriodAcceptData = allAcceptVolumeData.Where(x => allAcceptVolumeHeaders.Where(y => CheckBeforeTimePeriod(y.TO_DATE.Value, model)).Select(y => y.ID).Contains(x.HEADER_ID));
            var vendors = contracts.Select(x => x.Vendor).GroupBy(x => x.CODE).Select(x => x.First());
            var listItems = VendorMonitoringData(contracts,
                                                 projectStructs,
                                                 units,
                                                 inPeriodPlanCostData,
                                                 beforePlanCostData,
                                                 inPeriodVolumeData,
                                                 beforePeriodVolumeData,
                                                 inPeriodAcceptData,
                                                 beforePeriodAcceptData,
                                                 vendors).ToList()
                                                 .OrderBy(x => x.VendorCode).ThenBy(x => x.ProjectId).ThenBy(x => x.StructureOrder).ToList();
            foreach (var contractCode in listItems.GroupBy(x => x.ContractCode).Select(x => x.Key))
            {
                var firstIndex = listItems.FindIndex(x => x.ContractCode == contractCode);
                var allItemContracts = listItems.Where(x => x.ContractCode == contractCode);
                listItems.Insert(firstIndex, new VendorMonitoringReportData
                {
                    VendorCode = listItems.First(x => x.ContractCode == contractCode).VendorCode,
                    VendorName = listItems.First(x => x.ContractCode == contractCode).VendorName,
                    ContractCode = contractCode,
                    StructureName = "Tổng cộng",
                    IsSummary = true
                });
            }
            foreach (var vendorCode in listItems.Where(x => x.IsSummary).GroupBy(x => x.VendorCode).Select(x => x.Key))
            {
                var firstIndex = listItems.FindIndex(x => x.VendorCode == vendorCode);
                var allItems = listItems.Where(x => x.VendorCode == vendorCode && x.IsSummary);
                var vendorName = listItems.First(x => x.VendorCode == vendorCode).VendorName;
                listItems.Insert(firstIndex, new VendorMonitoringReportData
                {
                    VendorCode = vendorCode,
                    VendorName = vendorName,
                    StructureName = $"Tổng {vendorName}",
                    IsSummary = true
                });
            }
            listItems.Insert(0, new VendorMonitoringReportData
            {
                StructureName = $"TỔNG CỘNG",
                IsSummary = true
            });
            return listItems;
        }

        private static IEnumerable<VendorMonitoringReportData> VendorMonitoringData(IList<T_PS_CONTRACT> contracts,
                                                                                    IEnumerable<T_PS_PROJECT_STRUCT> projectStructs,
                                                                                    IList<T_MD_UNIT> units,
                                                                                    IEnumerable<T_PS_PLAN_COST> inPeriodPlanCostData,
                                                                                    IEnumerable<T_PS_PLAN_COST> beforePlanCostData,
                                                                                    IEnumerable<T_PS_VOLUME_WORK_DETAIL> inPeriodVolumeData,
                                                                                    IEnumerable<T_PS_VOLUME_WORK_DETAIL> beforePeriodVolumeData,
                                                                                    IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> inPeriodAcceptData,
                                                                                    IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> beforePeriodAcceptData,
                                                                                    IEnumerable<T_MD_VENDOR> vendors)
        {

            foreach (var vendor in vendors)
            {
                var orderedContracts = contracts.Where(x => x.VENDOR_CODE == vendor.CODE).OrderBy(x => x.PARENT_CODE);
                foreach (var contract in orderedContracts)
                {
                    foreach (var contractDetail in contract.Details)
                    {
                        var structureId = contractDetail.PROJECT_STRUCT_ID;
                        var projectStruct = projectStructs.FirstOrDefault(x => x.ID == structureId);
                        if (projectStruct != null)
                        {
                            var unit = units.FirstOrDefault(x => x.CODE == projectStruct.UNIT_CODE);

                            var inPeriodPlanVolume = inPeriodPlanCostData.Where(x => x.PROJECT_STRUCT_ID == structureId).ToList();
                            var endPeriodPlanVolume = beforePlanCostData.Where(x => x.PROJECT_STRUCT_ID == structureId).ToList();
                            var inPeriodWorkVolume = inPeriodVolumeData.Where(x => x.PROJECT_STRUCT_ID == structureId).ToList();
                            var inPeriodAcceptVolume = inPeriodAcceptData.Where(x => x.PROJECT_STRUCT_ID == structureId).ToList();
                            var endPeriodWorkVolume = beforePeriodVolumeData.Where(x => x.PROJECT_STRUCT_ID == structureId).ToList();
                            var endPeriodAcceptVolume = beforePeriodAcceptData.Where(x => x.PROJECT_STRUCT_ID == structureId).ToList();

                            yield return new VendorMonitoringReportData
                            {
                                Id = projectStruct?.ID,
                                ParentId = projectStruct?.PARENT_ID,
                                StructureCode = projectStruct?.GEN_CODE,
                                StructureName = projectStruct?.TEXT,
                                UnitName = unit?.NAME,
                                UnitCode = projectStruct?.UNIT_CODE,
                                ContractCode = contract.CONTRACT_NUMBER,
                                ContractPrice = contractDetail.UNIT_PRICE,
                                ContractVolume = contractDetail.VOLUME,
                                StructurePrice = projectStruct?.PRICE,
                                VendorName = vendor.NAME,
                                VendorCode = vendor.CODE,
                                IsSummary = false,
                                ProjectId = projectStruct?.PROJECT_ID ?? Guid.Empty,
                                StructureOrder = projectStruct?.C_ORDER ?? 0,

                                InPeriodWorkVolume = inPeriodWorkVolume.Sum(x => x.VALUE),
                                InPeriodAcceptVolume = inPeriodAcceptVolume.Sum(x => x.VALUE),
                                InPeriodPlanVolume = inPeriodPlanVolume.Sum(x => x.VALUE),
                                InPeriodWorkPrice = inPeriodWorkVolume.FirstOrDefault()?.PRICE,
                                InPeriodAcceptPrice = inPeriodAcceptVolume.FirstOrDefault()?.PRICE,

                                EndPeriodPlanVolume = endPeriodPlanVolume.Sum(x => x.VALUE),
                                EndPeriodAcceptVolume = endPeriodAcceptVolume.Sum(x => x.VALUE),
                                EndPeriodWorkVolume = endPeriodWorkVolume.Sum(x => x.VALUE),
                                EndPeriodWorkPrice = endPeriodWorkVolume.FirstOrDefault()?.PRICE,
                                EndPeriodAcceptPrice = endPeriodAcceptVolume.FirstOrDefault()?.PRICE,

                                InPeriodTotalWorkVolume = inPeriodWorkVolume.Sum(x => x.TOTAL),
                                InPeriodTotalAcceptVolume = inPeriodAcceptVolume.Sum(x => x.TOTAL),
                                EndPeriodTotalWorkVolume = endPeriodWorkVolume.Sum(x => x.TOTAL),
                                EndPeriodTotalAcceptVolume = endPeriodAcceptVolume.Sum(x => x.TOTAL),
                            };
                        }

                    }
                }
            }
            yield break;
        }

        private string GetProjectTimeTypeString(Guid projectId)
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
        internal void SetHeaderExportExcel(ref MemoryStream outFileHeaderStream, MemoryStream outFileStream, int numberPeriods, BaseReportModel model, int startCol)
        {
            try
            {
                outFileStream.Position = 0;
                IWorkbook templateWorkbook = new XSSFWorkbook(outFileStream);
                outFileStream.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);
                var allTimePeriods = GetAllInPeriodTimes(model, new List<Guid> { model.ProjectId }).OrderBy(x => x.C_ORDER).ToList();
                var timeType = GetProjectTimeTypeString(model.ProjectId);
                var rowHeaderPeriod = sheet.GetRow(6);
                var COL_PER_PERIOD = 7;
                var rowHeaderValue = sheet.GetRow(7);

                ReportUtilities.CreateCell(ref rowHeaderPeriod, numberPeriods * COL_PER_PERIOD + startCol + 1);
                ReportUtilities.CreateCell(ref rowHeaderValue, numberPeriods * COL_PER_PERIOD + startCol + 1);
                for (int i = 0; i < numberPeriods; i++)
                {
                    var timePeriod = allTimePeriods[i];
                    var cra = new CellRangeAddress(firstRow: 6, lastRow: 6, firstCol: i * COL_PER_PERIOD + startCol + 1, lastCol: i * COL_PER_PERIOD + startCol + COL_PER_PERIOD);
                    sheet.AddMergedRegion(cra);
                    for (int j = 1; j <= COL_PER_PERIOD; j++)
                    {
                        rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + j, i * COL_PER_PERIOD + startCol + j);
                        rowHeaderValue.Cells[startCol - COL_PER_PERIOD + j].CopyCellTo(i * COL_PER_PERIOD + startCol + j);
                        sheet.SetColumnWidth(i * COL_PER_PERIOD + startCol + j, sheet.GetColumnWidth(startCol - COL_PER_PERIOD + j));
                    }
                    rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 1, i * COL_PER_PERIOD + startCol + 1).SetCellValue($"{timeType.ToUpper()} {i + 1} ({timePeriod.START_DATE.ToString(Global.DateToStringFormat)}-{timePeriod.FINISH_DATE.ToString(Global.DateToStringFormat)})");

                }
                templateWorkbook.Write(outFileHeaderStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

        internal IEnumerable<ProjectDetailDataCost> GenerateProjectDetailDataReport(ProjectDetailDataCostModel model, bool isCustomer)
        {
            var project = GetProject(model.ProjectId);
            model.FromDate = project.START_DATE;
            model.ToDate = project.FINISH_DATE;

            var allTimePeriods = GetAllInPeriodTimes(model, new List<Guid> { model.ProjectId });
            var allTimePeriodIds = allTimePeriods.OrderBy(x => x.C_ORDER).Select(x => x.ID);

            var allWorkVolumeHeaders = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
                .Where(x => x.PROJECT_ID == model.ProjectId && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue() && x.IS_CUSTOMER == isCustomer)
                .ToList();
            var allWorkVolumeHeaderIds = allWorkVolumeHeaders.Select(x => x.ID);
            var allWorkVolumeData = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                .Where(x => allWorkVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();

            var allAcceptVolumeHeaders = UnitOfWork.Repository<VolumeAcceptRepo>().Queryable()
                .Where(x => x.PROJECT_ID == model.ProjectId && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue() && x.IS_CUSTOMER == isCustomer)
                .ToList();
            var allAcceptVolumeHeaderIds = allAcceptVolumeHeaders.Select(x => x.ID);
            var allAcceptVolumeData = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
                .Where(x => allAcceptVolumeHeaderIds.Contains(x.HEADER_ID))
                .ToList();

            var planCostData = UnitOfWork.Repository<PlanCostRepo>().Queryable()
                .Where(x => x.PROJECT_ID == model.ProjectId && x.IS_CUSTOMER == isCustomer)
                .ToList();

            var queryContractDetails = UnitOfWork.Repository<ContractDetailRepo>().Queryable();
            if (isCustomer)
            {
                if (model.ContractId.HasValue && model.ContractId != Guid.Empty)
                {
                    queryContractDetails = queryContractDetails.Where(x => x.CONTRACT_ID == model.ContractId);
                }
                queryContractDetails = queryContractDetails.Where(x => x.Contract.IS_SIGN_WITH_CUSTOMER);
            }
            else
            {
                if (!string.IsNullOrEmpty(model.Vendor))
                {
                    queryContractDetails = queryContractDetails.Where(x => x.Contract.VENDOR_CODE == model.Vendor);
                }
                queryContractDetails = queryContractDetails.Where(x => !x.Contract.IS_SIGN_WITH_CUSTOMER);
            }
            var contractDetails = queryContractDetails.Fetch(x => x.Contract).ToList();
            var projectStructIds = contractDetails.Select(x => x.PROJECT_STRUCT_ID);

            List<T_PS_PROJECT_STRUCT> projectStructs = new List<T_PS_PROJECT_STRUCT>();
            if (isCustomer)
            {
                projectStructs = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                                .Where(x => x.PROJECT_ID == model.ProjectId && x.TYPE != "ACTIVITY" && x.TYPE != "WBS")
                                .ToList();
            }
            else
            {
                projectStructs = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                                .Where(x => x.PROJECT_ID == model.ProjectId && x.TYPE != "BOQ")
                                .ToList();
            }

            var units = UnitOfWork.Repository<Repository.Implement.MD.UnitRepo>().GetAll();

            return from projectStruct in projectStructs.OrderBy(x => x.C_ORDER)
                   let planCosts = planCostData.Where(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                   let workVolumes = allWorkVolumeData.Where(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                   let acceptVolumes = allAcceptVolumeData.Where(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                   let contractDetail = contractDetails.Where(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                   select new ProjectDetailDataCost
                   {
                       Id = projectStruct.ID,
                       ParentId = projectStruct.PARENT_ID,
                       ContractCode = contractDetail.FirstOrDefault()?.Contract?.CONTRACT_NUMBER,
                       Type = projectStruct.TYPE,
                       ProjectStructureCode = projectStruct.GEN_CODE,
                       ProjectStructureName = projectStruct.TEXT,
                       VendorName = isCustomer ? null : contractDetail.FirstOrDefault()?.Contract?.Vendor?.NAME,
                       UnitName = projectStruct.UNIT_CODE,
                       Price = projectStruct.PRICE ?? 0,
                       ContractPrice = projectStruct.PRICE ?? 0,
                       DataCostPeriods = CalculatePeriod(allTimePeriods, planCosts, workVolumes, acceptVolumes, allWorkVolumeHeaders, allAcceptVolumeHeaders)
                   };
        }

        private IEnumerable<ProjectDetailDataCostPeriod> CalculatePeriod(IEnumerable<T_PS_TIME> allTimePeriodIds,
                                                                         IEnumerable<T_PS_PLAN_COST> planCosts,
                                                                         IEnumerable<T_PS_VOLUME_WORK_DETAIL> workVolumes,
                                                                         IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> acceptVolumes,
                                                                         IEnumerable<T_PS_VOLUME_WORK> allWorkVolumeHeaders,
                                                                         IEnumerable<T_PS_VOLUME_ACCEPT> allAcceptVolumeHeaders)
        {
            return from period in allTimePeriodIds
                   let periodPlanCosts = planCosts.Where(x => x.TIME_PERIOD_ID == period.ID)
                   let workVolumeHeaderPeriodIds = allWorkVolumeHeaders.Where(x => x.TO_DATE <= period.FINISH_DATE && x.TO_DATE >= period.START_DATE).Select(x => x.ID)
                   let periodWorkVolumes = workVolumes.Where(x => workVolumeHeaderPeriodIds.Contains(x.HEADER_ID))
                   let workAcceptHeaderPeriodIds = allAcceptVolumeHeaders.Where(x => x.TO_DATE <= period.FINISH_DATE && x.TO_DATE >= period.START_DATE).Select(x => x.ID)
                   let periodWorkAccepts = acceptVolumes.Where(x => workAcceptHeaderPeriodIds.Contains(x.HEADER_ID))
                   select new ProjectDetailDataCostPeriod
                   {
                       PeriodId = period.ID,
                       StartDate = period.START_DATE,
                       ToDate = period.FINISH_DATE,
                       Order = period.C_ORDER,
                       PlanVolume = periodPlanCosts.Sum(x => x.VALUE),
                       WorkedVolume = periodWorkVolumes.Sum(x => x.VALUE),
                       AcceptedVolume = periodWorkAccepts.Sum(x => x.VALUE),
                       WorkedPrice = periodWorkVolumes.LastOrDefault()?.PRICE,
                       AcceptedPrice = periodWorkAccepts.LastOrDefault()?.PRICE,
                   };
        }

        private bool CheckInTimePeriod(DateTime date, BaseReportModel model)
        {
            return date <= model.ToDate && date >= model.FromDate;
        }
        private bool CheckDuringYearTimePeriod(DateTime date, BaseReportModel model)
        {
            return date <= model.ToDate && date.Year == model.ToDate?.Year;
        }
        private bool CheckBeforeTimePeriod(DateTime date, BaseReportModel model)
        {
            return date <= model.ToDate;
        }
        private bool CheckBeforeStartTimePeriod(DateTime date, BaseReportModel model)
        {
            return date < model.FromDate;
        }
        internal T_PS_PROJECT GetProject(Guid projectId)
        {
            return CurrentRepository.Get(projectId);
        }

        private ICellStyle GetCellStyleNumber(IWorkbook templateWorkbook)
        {
            ICellStyle styleCellNumber = templateWorkbook.CreateCellStyle();
            styleCellNumber.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0");
            return styleCellNumber;
        }
        private ICellStyle GetCellStyleNumberDecimal(IWorkbook templateWorkbook)
        {
            ICellStyle styleCellNumber = templateWorkbook.CreateCellStyle();
            styleCellNumber.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0.000");
            return styleCellNumber;
        }
        private ICellStyle GetCellStylePercentage(IWorkbook templateWorkbook)
        {
            ICellStyle styleCellPercentage = templateWorkbook.CreateCellStyle();
            styleCellPercentage.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("0.000%");
            return styleCellPercentage;
        }
        internal void ExportReportExcel(ref MemoryStream outFileStream,
                                        string path,
                                        ConfigTemplateModel config,
                                        IList<IEnumerable<string>> data,
                                        IEnumerable<IEnumerable<string>> header)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);
                var styleCellNumber = GetCellStyleNumber(templateWorkbook);
                var styleCellNumberDecimal = GetCellStyleNumberDecimal(templateWorkbook);
                var styleCellPercentage = GetCellStylePercentage(templateWorkbook);

                InitHeader(sheet, config);

                var startRow = config.StartRow;

                for (int i = 0; i < data.Count(); i++)
                {
                    var dataRow = data[i];
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow++, dataRow.Count());
                    var isRowPercentageUnit = config.PercentageRowIndexes.Contains(i);
                    for (int j = 0; j < dataRow.Count(); j++)
                    {
                        var valueStr = dataRow.ElementAt(j);
                        valueStr = valueStr == null ? string.Empty : valueStr;
                        if (j >= config.StartColumnNumber && (!config.EndColumnNumber.HasValue || j <= config.EndColumnNumber.Value))
                        {
                            if (valueStr == "NaN")
                            {
                                valueStr = "0";
                            }
                            double value = 0;
                            var canParseNumber = double.TryParse(valueStr, out value);
                            var isHeaderVolume = header.Any(x => x.ToList()[j].Contains("KL") || x.ToList()[j].ToLower().Contains("khối lượng"));
                            rowCur.Cells[j].CellStyle = config.ColumnsPercentage.Contains(j)
                                || (isRowPercentageUnit && config.ColumnsVolume.Contains(j)) ?
                                styleCellPercentage :
                                isHeaderVolume ? styleCellNumberDecimal : styleCellNumber;
                            rowCur.Cells[j].SetCellValue(canParseNumber ? value : 0);
                        }
                        else
                        {
                            rowCur.Cells[j].SetCellValue(valueStr);
                        }
                        if (config.BoldRowIndexes.Contains(i))
                        {
                            var cellStyle = templateWorkbook.CreateCellStyle();
                            cellStyle.CloneStyleFrom(rowCur.Cells[j].CellStyle);
                            var font = templateWorkbook.CreateFont();
                            font.IsBold = true;
                            font.FontHeightInPoints = 11;
                            cellStyle.SetFont(font);
                            rowCur.Cells[j].CellStyle = cellStyle;
                        }
                    }
                }
                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

        internal void ExportResourceExcel(ref MemoryStream outFileStream,
                                        string path,
                                        ConfigTemplateModel config,
                                        IList<IEnumerable<string>> data,
                                        IEnumerable<IEnumerable<string>> header)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);            
                InitHeader(sheet, config);

                var startRow = config.StartRow;

                for (int i = 0; i < data.Count(); i++)
                {
                    var dataRow = data[i];                    
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow++, dataRow.Count());
                    for (int j = 0; j < dataRow.Count(); j++)
                    {
                        var valueStr = dataRow.ElementAt(j);                       
                       
                            rowCur.Cells[j].SetCellValue(valueStr);
                        
                        if (config.BoldRowIndexes.Contains(i))
                        {
                            var cellStyle = templateWorkbook.CreateCellStyle();
                            cellStyle.CloneStyleFrom(rowCur.Cells[j].CellStyle);
                            var font = templateWorkbook.CreateFont();
                            font.IsBold = true;
                            font.FontHeightInPoints = 11;
                            cellStyle.SetFont(font);
                            rowCur.Cells[j].CellStyle = cellStyle;
                        }
                    }
                }
                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

        internal void ExportExcelDataBOQ(ref MemoryStream outFileStream,
                                        string path, Guid projectId)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);
                var styleCellNumber = GetCellStyleNumber(templateWorkbook);
                var styleCellNumberDecimal = GetCellStyleNumberDecimal(templateWorkbook);

                var data = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.TYPE != "WBS" && x.TYPE != "ACTIVITY").OrderBy(x => x.C_ORDER).ToList();

                if (data.Count <= 1)
                {
                    this.State = false;
                    this.ErrorMessage = string.Format("<p style='font-size:24px;'>Cây cấu trúc đang không có dữ liệu! Vui lòng thêm dữ liệu trước khi xuất excel hoặc có thể chọn tải Template Import cây cấu trúc!</p>");
                    return;
                }
                var contract = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.CONTRACT_TYPE.Contains("KD")).FirstOrDefault();

                var startRow = 7;

                for (int i = 0; i < data.Count(); i++)
                {
                    var dataRow = data[i];
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow++, 11);
                    rowCur.Cells[0].SetCellValue(data[i]?.TYPE);
                    rowCur.Cells[1].SetCellValue(data[i]?.GEN_CODE);
                    rowCur.Cells[2].SetCellValue(data[i]?.TEXT);
                    rowCur.Cells[3].SetCellValue(data[i]?.START_DATE.ToString("dd/MM/yyyy"));
                    rowCur.Cells[4].SetCellValue(data[i]?.FINISH_DATE.ToString("dd/MM/yyyy"));
                    rowCur.Cells[5].SetCellValue(data[i]?.UNIT_CODE);
                    rowCur.Cells[6].SetCellValue(data[i]?.STATUS == "01" ? "Chưa bắt đầu" : data[i]?.STATUS == "02" ? "Đang thực hiện" : data[i]?.STATUS == "03" ? "Tạm dừng" : "Hoàn thành");

                    if (data[i].TYPE == "PROJECT" || data[i]?.QUANTITY == null || (decimal?)data[i]?.QUANTITY == 0)
                    {
                        rowCur.Cells[7].SetCellValue("");
                    }
                    else
                    {
                        rowCur.Cells[7].CellStyle = styleCellNumberDecimal;
                        rowCur.Cells[7].SetCellValue(data[i]?.UNIT_CODE == "%" ? (double)data[i]?.QUANTITY * 100 : (double)data[i]?.QUANTITY);
                    }

                    if (data[i].TYPE == "PROJECT" || data[i]?.PRICE == null || (decimal?)data[i]?.PRICE == 0)
                    {
                        rowCur.Cells[8].SetCellValue("");
                    }
                    else
                    {
                        rowCur.Cells[8].CellStyle = styleCellNumber;
                        rowCur.Cells[8].SetCellValue((double)data[i]?.PRICE);
                    }

                    if (data[i].TOTAL == 0 || data[i].TOTAL == null)
                    {
                        rowCur.Cells[9].SetCellValue("");
                    }
                    else
                    {
                        rowCur.Cells[9].CellStyle = styleCellNumber;
                        rowCur.Cells[9].SetCellValue((double)data[i].TOTAL);
                    }


                    rowCur.Cells[10].SetCellValue(data[i].TYPE == "PROJECT" ? null : contract?.CONTRACT_NUMBER);
                }

                ISheet sheetDVT = templateWorkbook.GetSheetAt(1);

                var dataDVT = UnitOfWork.Repository<UnitRepo>().GetAll().ToList();
                var startRowDVT = 2;

                for (int i = 0; i < dataDVT.Count(); i++)
                {
                    IRow rowCur = ReportUtilities.CreateRow(ref sheetDVT, startRowDVT++, 2);
                    rowCur.Cells[0].SetCellValue(dataDVT[i].CODE);
                    rowCur.Cells[1].SetCellValue(dataDVT[i].NAME);
                }

                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

        internal void ExportExcelDataCHIPHI(ref MemoryStream outFileStream,
                                        string path, Guid projectId)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                fs.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);
                var styleCellNumber = GetCellStyleNumber(templateWorkbook);
                var styleCellNumberDecimal = GetCellStyleNumberDecimal(templateWorkbook);

                var data = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.TYPE != "BOQ").OrderBy(x => x.C_ORDER).ToList();

                if (data.Count <= 1)
                {
                    this.State = false;
                    this.ErrorMessage = string.Format("<p style='font-size:24px;'>Cây cấu trúc đang không có dữ liệu! Vui lòng thêm dữ liệu trước khi xuất excel hoặc có thể chọn tải Template Import cây cấu trúc!</p>");
                    return;
                }

                var startRow = 7;

                for (int i = 0; i < data.Count(); i++)
                {
                    var dataRow = data[i];
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow++, 13);
                    rowCur.Cells[0].SetCellValue(data[i]?.TYPE);
                    rowCur.Cells[1].SetCellValue(data[i]?.GEN_CODE);
                    rowCur.Cells[2].SetCellValue(data[i]?.TEXT);
                    rowCur.Cells[3].SetCellValue(data[i]?.START_DATE.ToString("dd/MM/yyyy"));
                    rowCur.Cells[4].SetCellValue(data[i]?.FINISH_DATE.ToString("dd/MM/yyyy"));
                    rowCur.Cells[5].SetCellValue(data[i]?.UNIT_CODE);
                    rowCur.Cells[6].SetCellValue(data[i]?.STATUS == "01" ? "Chưa bắt đầu" : data[i]?.STATUS == "02" ? "Đang thực hiện" : data[i]?.STATUS == "03" ? "Tạm dừng" : "Hoàn thành");

                    if (data[i].TYPE == "PROJECT" || data[i]?.QUANTITY == null || (decimal?)data[i]?.QUANTITY == 0)
                    {
                        rowCur.Cells[7].SetCellValue("");
                    }
                    else
                    {
                        rowCur.Cells[7].CellStyle = styleCellNumberDecimal;
                        rowCur.Cells[7].SetCellValue(data[i]?.UNIT_CODE == "%" ? (double)data[i]?.QUANTITY * 100 : (double)data[i]?.QUANTITY);
                    }

                    if (data[i].TYPE == "PROJECT" || data[i]?.PRICE == null || (decimal?)data[i]?.PRICE == 0)
                    {
                        rowCur.Cells[8].SetCellValue("");
                    }
                    else
                    {
                        rowCur.Cells[8].CellStyle = styleCellNumber;
                        rowCur.Cells[8].SetCellValue((double)data[i]?.PRICE);
                    }

                    if (data[i].TOTAL == 0 || data[i].TOTAL == null)
                    {
                        rowCur.Cells[9].SetCellValue("");
                    }
                    else
                    {
                        rowCur.Cells[9].CellStyle = styleCellNumber;
                        rowCur.Cells[9].SetCellValue((double)data[i].TOTAL);
                    }


                    rowCur.Cells[10].SetCellValue(data[i].TYPE == "WBS" ? data[i]?.Wbs?.ReferenceBoq?.GEN_CODE : data[i].TYPE == "ACTIVITY" ? data[i]?.Activity?.ReferenceBoq?.GEN_CODE : null);

                    var contract = UnitOfWork.Repository<ContractDetailRepo>().Queryable().Where(x => x.PROJECT_STRUCT_ID == data[i].ID).FirstOrDefault();

                    rowCur.Cells[11].SetCellValue(contract == null ? null : contract?.Contract?.Vendor?.NAME);
                    rowCur.Cells[12].SetCellValue(contract == null ? null : contract?.Contract?.CONTRACT_NUMBER);

                }

                ISheet sheetDVT = templateWorkbook.GetSheetAt(1);

                var dataDVT = UnitOfWork.Repository<UnitRepo>().GetAll().ToList();
                var startRowDVT = 2;

                for (int i = 0; i < dataDVT.Count(); i++)
                {
                    IRow rowCur = ReportUtilities.CreateRow(ref sheetDVT, startRowDVT++, 2);
                    rowCur.Cells[0].SetCellValue(dataDVT[i].CODE);
                    rowCur.Cells[1].SetCellValue(dataDVT[i].NAME);
                }

                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

        internal void ExportExcelTemplateStruct(ref MemoryStream outFileStream, string path)
        {
            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                IWorkbook templateWorkbook;
                templateWorkbook = new XSSFWorkbook(fs);
                fs.Close();

                ISheet sheet = templateWorkbook.GetSheetAt(1);

                var data = UnitOfWork.Repository<UnitRepo>().GetAll().ToList();
                var startRow = 2;

                for (int i = 0; i < data.Count(); i++)
                {
                    var dataRow = data[i];
                    IRow rowCur = ReportUtilities.CreateRow(ref sheet, startRow++, 2);
                    rowCur.Cells[0].SetCellValue(data[i].CODE);
                    rowCur.Cells[1].SetCellValue(data[i].NAME);
                }
                templateWorkbook.Write(outFileStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }


        private void InitHeader(ISheet sheet, ConfigTemplateModel config)
        {
            foreach (var param in config.HeaderParams)
            {
                var row = ReportUtilities.CreateRow(ref sheet, param.Key.Y, param.Key.X);
                ReportUtilities.CreateCell(ref row, param.Key.X);
                row.Cells[param.Key.X].SetCellValue(param.Value);
            }
        }

        private void CreateProjectSAP(List<T_PS_PROJECT_STRUCT> lstProjectStruct)
        {
            try
            {
                var hasError = lstProjectStruct.Where(x =>
                (x.PRICE + x.QUANTITY > 0 && x.QUANTITY == null && x.UNIT_CODE == null && x.TYPE != "BOQ") ||
                (x.PRICE + x.QUANTITY > 0 && x.QUANTITY == 0 && x.UNIT_CODE == null && x.TYPE != "BOQ") ||
                (x.PRICE + x.QUANTITY > 0 && x.PRICE == null && x.UNIT_CODE == null && x.TYPE != "BOQ") ||
                (x.PRICE + x.QUANTITY > 0 && x.PRICE == 0 && x.UNIT_CODE == null && x.TYPE != "BOQ"))
               .ToList();

                var genCode = "";
                for (var i = 0; i < hasError.Count(); i++)
                {
                    genCode += hasError[i].GEN_CODE + ", ";
                }
                if (hasError.FirstOrDefault() != null)
                {
                    ErrorMessage = $"Vui lòng nhập thêm ĐVT khi chỉ nhập Đơn giá hoặc Khối lượng tại các hạng mục {genCode}!";
                    State = false;
                    return;
                }
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                var projectProfile = UnitOfWork.GetSession().Query<T_MD_PROJECT_PROFILE>().FirstOrDefault(
                    x => x.COMPANY_CODE == this.ObjDetail.DonVi.COMPANY_CODE && x.PROJECT_TYPE == this.ObjDetail.TYPE);

                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                            systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new Create_Project_Function();
                    var project = new ZST_BAPIPROJ()
                    {
                        COMPANY_CODE = this.ObjDetail.DonVi.COMPANY_CODE,
                        COST_CENTER_CODE = this.ObjDetail.PhongBan.COST_CENTER_CODE,
                        PROJECT_CODE = this.ObjDetail.CODE,
                        PROJECT_NAME = this.ObjDetail.NAME,
                        FINISH_DATE = this.ObjDetail.FINISH_DATE,
                        START_DATE = this.ObjDetail.START_DATE,
                        PROJECT_PROFILE = projectProfile?.PROJECT_PROFILE
                    };

                    var lstWbs = new List<ZST_BAPIWBS>();
                    foreach (var item in lstProjectStruct.Where(x => x.TYPE == "WBS").ToList())
                    {
                        lstWbs.Add(new ZST_BAPIWBS()
                        {
                            ACTUAL_FINISH_DATE = item.Wbs.ACTUAL_FINISH_DATE,
                            ACTUAL_START_DATE = item.Wbs.ACTUAL_START_DATE,
                            FINISH_DATE = item.FINISH_DATE,
                            START_DATE = item.START_DATE,
                            PARENT_CODE = (item.Parent?.GEN_CODE == item?.GEN_CODE ? "" : item.Parent?.GEN_CODE),
                            WBS_CODE = item.GEN_CODE,
                            WBS_NAME = item.TEXT
                        });
                    }

                    var lstActivity = new List<ZST_BAPIACTI>();
                    foreach (var item in lstProjectStruct.Where(x => x.TYPE == "ACTIVITY").ToList())
                    {
                        lstActivity.Add(new ZST_BAPIACTI()
                        {
                            ACTUAL_FINISH_DATE = item.Activity.ACTUAL_FINISH_DATE,
                            ACTUAL_START_DATE = item.Activity.ACTUAL_START_DATE,
                            FINISH_DATE = item.FINISH_DATE,
                            START_DATE = item.START_DATE,
                            PARENT_CODE = item.Parent?.GEN_CODE,
                            ACTIVITY_CODE = item.GEN_CODE,
                            ACTIVITY_NAME = item.TEXT,
                            UNIT_CODE = item.UNIT_CODE,
                            QUANTITY = item.UNIT_CODE == "%" ? item.QUANTITY * 100 : item.QUANTITY,
                            CONTROL_KEY = "PS02",
                            PUR_GROUP = this.ObjDetail.PUR_GROUP
                        });
                    }

                    functionSAP.Parameters = new
                    {
                        PROJECT = project,
                        WBS = lstWbs,
                        ACTIVITY = lstActivity
                    };

                    conn.ExecuteFunction(functionSAP);
                    var output = functionSAP.Result.GetOutput<BAPIRET2>("RETURN");
                    this.State = true;
                    if (output.TYPE == "E")
                    {
                        this.State = false;
                        this.ErrorMessage = output.MESSAGE;
                    }

                    if (this.State)
                    {
                        try
                        {
                            this.UnitOfWork.Clear();
                            UnitOfWork.BeginTransaction();
                            UnitOfWork.GetSession().Query<T_PS_PROJECT>().Where(x => x.ID == this.ObjDetail.ID)
                                .Update(x => new T_PS_PROJECT()
                                {
                                    IS_CREATE_ON_SAP = true
                                });
                            UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => x.PROJECT_ID == this.ObjDetail.ID)
                                .Update(x => new T_PS_PROJECT_STRUCT()
                                {
                                    IS_CREATE_ON_SAP = true
                                });
                            UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_ID == this.ObjDetail.ID).Delete();
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
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        private void UpdateProjectSAP(string action, List<T_PS_PROJECT_STRUCT> lstProjectStruct)
        {
            try
            {
                var hasError = lstProjectStruct.Where(x =>
                (x.PRICE + x.QUANTITY > 0 && x.QUANTITY == null && x.UNIT_CODE == null && x.TYPE != "BOQ") ||
                (x.PRICE + x.QUANTITY > 0 && x.QUANTITY == 0 && x.UNIT_CODE == null && x.TYPE != "BOQ") ||
                (x.PRICE + x.QUANTITY > 0 && x.PRICE == null && x.UNIT_CODE == null && x.TYPE != "BOQ") ||
                (x.PRICE + x.QUANTITY > 0 && x.PRICE == 0 && x.UNIT_CODE == null && x.TYPE != "BOQ"))
               .ToList();

                var genCode = "";
                for (var i = 0; i < hasError.Count(); i++)
                {
                    genCode += hasError[i].GEN_CODE + ", ";
                }
                if (hasError.FirstOrDefault() != null)
                {
                    ErrorMessage = $"Vui lòng nhập thêm ĐVT khi chỉ nhập Đơn giá hoặc Khối lượng tại các hạng mục {genCode}!";
                    State = false;
                    return;
                }
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                var lstStructChange = UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(
                        x => x.PROJECT_ID == ObjDetail.ID && x.ACTION == action
                    ).ToList();

                if (lstStructChange.Count == 0)
                {
                    return;
                }

                var lstStructIdChange = lstStructChange.Select(x => x.PROJECT_STRUCT_ID).ToList();

                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                            systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new Update_Project_Function();
                    var structIdOfProject = lstProjectStruct.FirstOrDefault(x => x.TYPE == "PROJECT");

                    var project = new ZST_BAPIPROJ()
                    {
                        PROJECT_CODE = this.ObjDetail.CODE,
                    };
                    if (lstStructChange.Count(x => x.PROJECT_STRUCT_ID == structIdOfProject.ID) > 0)
                    {
                        project.COMPANY_CODE = this.ObjDetail.DonVi.COMPANY_CODE;
                        project.COST_CENTER_CODE = this.ObjDetail.DonVi.COST_CENTER_CODE;
                        project.PROJECT_NAME = this.ObjDetail.NAME;
                        project.FINISH_DATE = this.ObjDetail.FINISH_DATE;
                        project.START_DATE = this.ObjDetail.START_DATE;
                    }

                    var lstWbs = new List<ZST_BAPIWBS>();
                    foreach (var item in lstProjectStruct.Where(x => x.TYPE == "WBS" && lstStructIdChange.Contains(x.ID)).ToList())
                    {
                        lstWbs.Add(new ZST_BAPIWBS()
                        {
                            ACTUAL_FINISH_DATE = item.Wbs.ACTUAL_FINISH_DATE,
                            ACTUAL_START_DATE = item.Wbs.ACTUAL_START_DATE,
                            FINISH_DATE = item.FINISH_DATE,
                            START_DATE = item.START_DATE,
                            PARENT_CODE = (item.Parent?.GEN_CODE == item.GEN_CODE ? "" : item.Parent?.GEN_CODE),
                            WBS_CODE = item.GEN_CODE,
                            WBS_NAME = item.TEXT
                        });
                    }

                    var lstActivity = new List<ZST_BAPIACTI>();
                    foreach (var item in lstProjectStruct.Where(x => x.TYPE == "ACTIVITY" && lstStructIdChange.Contains(x.ID)).ToList())
                    {
                        lstActivity.Add(new ZST_BAPIACTI()
                        {
                            ACTUAL_FINISH_DATE = item.Activity.ACTUAL_FINISH_DATE,
                            ACTUAL_START_DATE = item.Activity.ACTUAL_START_DATE,
                            FINISH_DATE = item.FINISH_DATE,
                            START_DATE = item.START_DATE,
                            PARENT_CODE = item.Parent?.GEN_CODE,
                            ACTIVITY_CODE = item.GEN_CODE,
                            ACTIVITY_NAME = item.TEXT,
                            UNIT_CODE = item.UNIT_CODE,
                            QUANTITY = item.UNIT_CODE == "%" ? item.QUANTITY * 100 : item.QUANTITY,
                            CONTROL_KEY = "PS02",
                            PUR_GROUP = this.ObjDetail.PUR_GROUP
                        });
                    }

                    functionSAP.Parameters = new
                    {
                        PROJECT = project,
                        ADD = action == "A" ? "X" : "",
                        CHANGE = action == "U" ? "X" : "",
                        DELETE = action == "D" ? "X" : "",
                        WBS = lstWbs,
                        ACTIVITY = lstActivity
                    };

                    conn.ExecuteFunction(functionSAP);
                    var output = functionSAP.Result.GetTable<BAPIRET2>("EX_RETURN");
                    this.State = true;
                    if (output.Where(x => x.TYPE == "E").Count() > 0)
                    {
                        this.State = false;
                        var error = "";
                        foreach (var e in output.Where(x => x.TYPE == "E"))
                        {
                            error += e.MESSAGE + " - ";
                        }
                        this.ErrorMessage = "Cập nhật thông tin lên SAP không thành công! " + error + "(Lưu ý đóng các màn hình chỉnh sửa dự án này trên SAP ERP trước khi nhấn nút Tạo mới | Lưu | Phê duyệt)";
                        //if(action == "A")
                        //{
                        //    foreach (var item in lstStructChange)
                        //    {
                        //        UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => x.ID == item.PROJECT_STRUCT_ID).Delete();
                        //        UnitOfWork.GetSession().Query<T_PS_CONTRACT_DETAIL>().Where(x => x.PROJECT_STRUCT_ID == item.PROJECT_STRUCT_ID).Delete();
                        //    }
                        //}
                    }

                    if (action == "U")
                    {
                        var functionOrderSAP = new Update_Order_Project_Function();
                        functionOrderSAP.Parameters = new
                        {
                            PROJECT = project,
                            WBS = lstWbs,
                            ACTIVITY = lstActivity
                        };
                        conn.ExecuteFunction(functionOrderSAP);

                        var outputOrder = functionOrderSAP.Result.GetTable<BAPIRET2>("EX_RETURN");
                        this.State = true;
                        if (outputOrder.Where(x => x.TYPE == "E").Count() > 0)
                        {
                            this.State = false;
                            var error = "";
                            foreach (var e in outputOrder.Where(x => x.TYPE == "E"))
                            {
                                error += e.MESSAGE + " - ";
                            }
                            this.ErrorMessage = "Cập nhật thông tin lên SAP không thành công! " + error + "(Lưu ý đóng các màn hình chỉnh sửa dự án này trên SAP ERP trước khi nhấn nút Tạo mới | Lưu | Phê duyệt)";
                            //if(action == "A")
                            //{
                            //    foreach (var item in lstStructChange)
                            //    {
                            //        UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => x.ID == item.PROJECT_STRUCT_ID).Delete();
                            //        UnitOfWork.GetSession().Query<T_PS_CONTRACT_DETAIL>().Where(x => x.PROJECT_STRUCT_ID == item.PROJECT_STRUCT_ID).Delete();
                            //    }
                            //}
                        }
                    }
                }

                if (this.State)
                {
                    try
                    {
                        UnitOfWork.BeginTransaction();

                        if (action == "A")
                        {
                            UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => lstStructIdChange.Contains(x.ID))
                            .Update(x => new T_PS_PROJECT_STRUCT()
                            {
                                IS_CREATE_ON_SAP = true
                            });
                        }

                        foreach (var item in lstStructChange)
                        {
                            UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == item.PROJECT_STRUCT_ID).Delete();
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
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
                this.ErrorMessage = "Cập nhật thông tin lên SAP không thành công!" + ex;
            }


        }

        internal string GenProjectCode()
        {
            string genCode = "";
            var findDonVi = UnitOfWork.GetSession().Query<T_AD_ORGANIZE>().FirstOrDefault(x => x.PKID == this.ObjDetail.DON_VI);
            if (findDonVi == null)
            {
                return "";
            }
            var findProfile = UnitOfWork.GetSession().Query<T_MD_PROJECT_PROFILE>().FirstOrDefault(
                x => x.COMPANY_CODE == findDonVi.COMPANY_CODE && x.PROJECT_TYPE == this.ObjDetail.TYPE);
            if (findProfile == null)
            {
                return "";
            }
            var findNumberRange = UnitOfWork.GetSession().Query<T_MD_NUMBER_RANGE>().FirstOrDefault(
                x => x.CHARACTER == findProfile.FIRST_CHARACTER);
            if (findNumberRange == null)
            {
                return "";
            }
            genCode = $"{findProfile.FIRST_CHARACTER}.{string.Format("{0:0000}", ++findNumberRange.CURRENT_NUMBER)}";
            try
            {
                UnitOfWork.BeginTransaction();
                UnitOfWork.GetSession().Update(findNumberRange);
                UnitOfWork.Commit();
            }
            catch
            {
                UnitOfWork.Rollback();
                genCode = "";
            }
            return genCode;
        }

        public void Create(HttpRequestBase Request, List<string> lstLink)
        {
            this.ObjDetail.CODE = this.GenProjectCode();

            if (this.ObjDetail.DON_VI == null || this.ObjDetail.PHONG_BAN == null)
            {
                this.State = false;
                this.ErrorMessage = "Công ty phụ trách và phòng ban phụ trách bắt buộc nhập!";
                return;
            }

            if (string.IsNullOrEmpty(this.ObjDetail.CODE))
            {
                this.State = false;
                this.ErrorMessage = "Quá trình sinh mã project bị lỗi. Nguyên nhân có thể do chưa cấu hình danh mục Project Profile cho đơn vị phụ trách vừa chọn!";
                return;
            }

            if (this.ObjDetail.CUSTOMER_CODE == null)
            {
                this.State = false;
                this.ErrorMessage = "Khách hàng bắt buộc nhập!";
                return;
            }
            if (this.ObjDetail.TIME_TYPE == null)
            {
                this.State = false;
                this.ErrorMessage = "Kỳ thời gian bắt buộc nhập!";
                return;
            }

            if (this.ObjDetail.FINISH_DATE <= this.ObjDetail.START_DATE)
            {
                this.State = false;
                this.ErrorMessage = "Ngày kết thúc dự án phải lớn hơn ngày bắt đầu!";
                return;
            }

            if (this.ObjDetail.PROJECT_LEVEL_CODE == null)
            {
                this.State = false;
                this.ErrorMessage = "Cấp dự án bắt buộc nhập!";
                return;
            }

            if (this.ObjDetail.NGAY_QUYET_TOAN == null || this.ObjDetail.HAN_BAO_HANH == null)
            {
                this.State = false;
                this.ErrorMessage = "Ngày quyết toán và ngày hết hạn bảo hành bắt buộc nhập!";
                return;
            }

            if (this.ObjDetail.FINISH_DATE >= this.ObjDetail.NGAY_QUYET_TOAN)
            {
                this.State = false;
                this.ErrorMessage = "Ngày quyết toán phải lớn hơn ngày kết thúc dự án!";
                return;
            }
            if (this.ObjDetail.NGAY_QUYET_TOAN >= this.ObjDetail.HAN_BAO_HANH)
            {
                this.State = false;
                this.ErrorMessage = "Ngày hết hạn bảo hành phải lớn hơn ngày quyết toán!";
                return;
            }

            if (this.ObjDetail.GIAM_DOC_DU_AN == null || this.ObjDetail.PROJECT_OWNER == null || this.ObjDetail.QUAN_TRI_DU_AN == null || this.ObjDetail.QUAN_LY_HOP_DONG == null || this.ObjDetail.PHU_TRACH_CUNG_UNG == null)
            {
                this.State = false;
                this.ErrorMessage = "Lãnh đạo phụ trách, PM dự án, Người phụ trách (SM), Phụ trách cung ứng, Quản lý hợp đồng bắt buộc nhập!";
                return;
            }

            this.ObjDetail.ID = Guid.NewGuid();
            this.ObjDetail.STATUS = ProjectStatus.KHOI_TAO.GetValue();
            this.ObjDetail.REFERENCE_FILE_ID = Guid.NewGuid();
            this.ObjDetail.STATUS_STRUCT_PLAN = ProjectStructureProgressStatus.KHOI_TAO.GetValue();
            this.ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;

            if (this.CheckExist(x => x.CODE == this.ObjDetail.CODE))
            {
                this.State = false;
                this.ErrorMessage = "Mã dự án đã tồn tại";
                return;
            }

            if (this.ObjDetail.NGAY_QUYET_TOAN == null || this.ObjDetail.HAN_BAO_HANH == null)
            {
                this.State = false;
                this.ErrorMessage = "Ngày quyết toán và ngày hết hạn bảo hành bắt buộc nhập!";
                return;
            }

            try
            {
                var lstFileStream = new List<FILE_STREAM>();
                for (int i = 0; i < Request.Files.AllKeys.Length; i++)
                {
                    var file = Request.Files[i];
                    var code = Guid.NewGuid();
                    lstFileStream.Add(new FILE_STREAM()
                    {
                        PKID = code,
                        FILE_OLD_NAME = file.FileName,
                        FILE_NAME = code + Path.GetExtension(file.FileName),
                        FILE_EXT = Path.GetExtension(file.FileName),
                        FILE_SIZE = file.ContentLength,
                        FILESTREAM = Request.Files[i]
                    });
                }
                FileStreamService.InsertFile(lstFileStream);

                UnitOfWork.BeginTransaction();
                if (lstLink != null)
                {
                    foreach (var link in lstLink)
                    {
                        if (!string.IsNullOrWhiteSpace(link))
                        {
                            UnitOfWork.GetSession().Save(new T_CM_REFERENCE_LINK()
                            {
                                ID = Guid.NewGuid(),
                                REFERENCE_ID = this.ObjDetail.REFERENCE_FILE_ID.Value,
                                LINK = link,
                                CREATE_BY = ProfileUtilities.User.USER_NAME
                            });
                        }
                    }
                }

                foreach (var item in lstFileStream)
                {
                    var referenceFile = new T_CM_REFERENCE_FILE()
                    {
                        REFERENCE_ID = this.ObjDetail.REFERENCE_FILE_ID.Value,
                        FILE_ID = item.PKID,
                        CREATE_BY = ProfileUtilities.User.USER_NAME
                    };

                    this.UnitOfWork.GetSession().Save(referenceFile);

                }

                var projectStruct = (T_PS_PROJECT_STRUCT)ObjDetail;
                projectStruct.ID = Guid.NewGuid();
                projectStruct.GEN_CODE = this.ObjDetail.CODE;
                projectStruct.ACTIVE = true;
                UnitOfWork.Repository<ProjectStructRepo>().Create(projectStruct);

                Dictionary<string, string> dicUser = new Dictionary<string, string>();
                dicUser.Add(this.ObjDetail.CREATE_BY, "SM");
                if (!string.IsNullOrWhiteSpace(this.ObjDetail.GIAM_DOC_DU_AN))
                {
                    if (dicUser.ContainsKey(this.ObjDetail.GIAM_DOC_DU_AN))
                    {
                        if (!dicUser[this.ObjDetail.GIAM_DOC_DU_AN].Contains("PD"))
                        {
                            dicUser[this.ObjDetail.GIAM_DOC_DU_AN] = dicUser[this.ObjDetail.GIAM_DOC_DU_AN] + ",PD";
                        }
                    }
                    else
                    {
                        dicUser.Add(this.ObjDetail.GIAM_DOC_DU_AN, "PD");
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.ObjDetail.QUAN_TRI_DU_AN))
                {
                    if (dicUser.ContainsKey(this.ObjDetail.QUAN_TRI_DU_AN))
                    {
                        if (!dicUser[this.ObjDetail.QUAN_TRI_DU_AN].Contains("PM"))
                        {
                            dicUser[this.ObjDetail.QUAN_TRI_DU_AN] = dicUser[this.ObjDetail.QUAN_TRI_DU_AN] + ",PM";
                        }
                    }
                    else
                    {
                        dicUser.Add(this.ObjDetail.QUAN_TRI_DU_AN, "PM");
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.ObjDetail.PHU_TRACH_CUNG_UNG))
                {
                    if (dicUser.ContainsKey(this.ObjDetail.PHU_TRACH_CUNG_UNG))
                    {
                        if (!dicUser[this.ObjDetail.PHU_TRACH_CUNG_UNG].Contains("CU"))
                        {
                            dicUser[this.ObjDetail.PHU_TRACH_CUNG_UNG] = dicUser[this.ObjDetail.PHU_TRACH_CUNG_UNG] + ",CU";
                        }
                    }
                    else
                    {
                        dicUser.Add(this.ObjDetail.PHU_TRACH_CUNG_UNG, "CU");
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.ObjDetail.QUAN_LY_HOP_DONG))
                {
                    if (dicUser.ContainsKey(this.ObjDetail.QUAN_LY_HOP_DONG))
                    {
                        if (!dicUser[this.ObjDetail.QUAN_LY_HOP_DONG].Contains("QLHD"))
                        {
                            dicUser[this.ObjDetail.QUAN_LY_HOP_DONG] = dicUser[this.ObjDetail.QUAN_LY_HOP_DONG] + ",QLHD";
                        }
                    }
                    else
                    {
                        dicUser.Add(this.ObjDetail.QUAN_LY_HOP_DONG, "QLHD");
                    }
                }

                if (!string.IsNullOrWhiteSpace(this.ObjDetail.PROJECT_OWNER))
                {
                    if (dicUser.ContainsKey(this.ObjDetail.PROJECT_OWNER))
                    {
                        if (!dicUser[this.ObjDetail.PROJECT_OWNER].Contains("SM"))
                        {
                            dicUser[this.ObjDetail.PROJECT_OWNER] = dicUser[this.ObjDetail.PROJECT_OWNER] + ",SM";
                        }
                    }
                    else
                    {
                        dicUser.Add(this.ObjDetail.PROJECT_OWNER, "SM");
                    }
                }

                foreach (var item in dicUser)
                {
                    UnitOfWork.GetSession().Save(new T_PS_RESOURCE()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = this.ObjDetail.ID,
                        USER_NAME = item.Key,
                        PROJECT_ROLE_ID = item.Value,
                        FROM_DATE = ObjDetail.START_DATE,
                        TO_DATE = ObjDetail.FINISH_DATE
                    });
                }

                this.CurrentRepository.Create(this.ObjDetail);

                UnitOfWork.Commit();

                AuthorizeService authorizeService = new AuthorizeService();
                authorizeService.GetInfoUser(ProfileUtilities.User.USER_NAME);
                if (!authorizeService.ObjUser.IS_IGNORE_USER)
                {
                    ProfileUtilities.User.IS_IGNORE_USER = false;
                    authorizeService.GetUserRight();
                    authorizeService.GetUserProjectRight();
                    if (authorizeService.ListUserRight.Select(x => x.CODE).Contains("R0"))
                    {
                        ProfileUtilities.User.IS_IGNORE_USER = true;
                    }
                }
                ProfileUtilities.UserRight = authorizeService.ListUserRight;
                ProfileUtilities.UserProjectRight = authorizeService.ListUserProjectRight;
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }

        public override void Update()
        {
            base.Update();
            var projectStruct = UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>()
            .FirstOrDefault(x => x.TYPE == ProjectEnum.PROJECT.ToString() && x.PROJECT_ID == this.ObjDetail.ID);
            var old_FROM_DATE = projectStruct.FINISH_DATE;
            try
            {
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var currentProjectTimePeriods = GetProjectPeriods(new List<Guid> { ObjDetail.ID });
                if (projectStruct.FINISH_DATE > ObjDetail.FINISH_DATE && currentProjectTimePeriods.Count() > 0)
                {
                    State = false;
                    ErrorMessage = "Ngày kết thúc dự án không được nhỏ hơn ngày kết thúc hiện tại.";
                    this.ObjDetail.FINISH_DATE = old_FROM_DATE;
                    UnitOfWork.GetSession().Update(projectStruct);
                    UnitOfWork.Commit();
                    return;
                }
                else if (ObjDetail.FINISH_DATE >= ObjDetail.NGAY_QUYET_TOAN)
                {
                    State = false;
                    ErrorMessage = "Ngày quyết toán phải lớn hơn ngày kết thúc dự án!";
                    return;
                }
                else if (ObjDetail.NGAY_QUYET_TOAN >= ObjDetail.HAN_BAO_HANH)
                {
                    State = false;
                    ErrorMessage = "Ngày bảo hành phải lớn hơn ngày quyết toán!";
                    return;
                }
                else if (ObjDetail.PHU_TRACH_CUNG_UNG == null || ObjDetail.QUAN_LY_HOP_DONG == null)
                {
                    State = false;
                    ErrorMessage = "Phụ trách cung ứng và quản lý hợp đồng bắt buộc nhập!";
                    return;
                }
                else if (currentProjectTimePeriods.Count() > 0)
                {
                    // check range end period
                    IList<TimePeriod> timePeriods = new List<TimePeriod>();
                    if (ObjDetail.TIME_TYPE == ProjectTimeTypeEnum.WEEK.GetValue())
                    {
                        timePeriods = SMOUtilities.GenerateTimeWeek(ObjDetail.START_DATE, ObjDetail.FINISH_DATE, ObjDetail.NGAY_QUYET_TOAN, ObjDetail.HAN_BAO_HANH);
                    }
                    else if (ObjDetail.TIME_TYPE == ProjectTimeTypeEnum.MONTH.GetValue())
                    {
                        timePeriods = SMOUtilities.GenerateTimeMonth(ObjDetail.START_DATE, ObjDetail.FINISH_DATE, ObjDetail.NGAY_QUYET_TOAN, ObjDetail.HAN_BAO_HANH);
                    }
                    var lastProjectTimePeriod = currentProjectTimePeriods.OrderBy(x => x.FINISH_DATE).ToList();
                    var projectTimeRepo = UnitOfWork.Repository<ProjectTimeRepo>();
                    if (currentProjectTimePeriods.Count() == timePeriods.Count)
                    {
                        projectTimeRepo.Queryable()
                                .Where(x => x.ID == lastProjectTimePeriod[lastProjectTimePeriod.Count() - 3].ID)
                                .Update(x => new T_PS_TIME
                                {
                                    FINISH_DATE = ObjDetail.FINISH_DATE,
                                    YEAR = ObjDetail.FINISH_DATE.Year,
                                    MONTH = ObjDetail.FINISH_DATE.Month,
                                    UPDATE_BY = currentUser
                                });
                        projectTimeRepo.Queryable()
                                .Where(x => x.ID == lastProjectTimePeriod[lastProjectTimePeriod.Count() - 2].ID)
                                .Update(x => new T_PS_TIME
                                {
                                    START_DATE = ObjDetail.NGAY_QUYET_TOAN ?? ObjDetail.FINISH_DATE,
                                    FINISH_DATE = ObjDetail.NGAY_QUYET_TOAN ?? ObjDetail.FINISH_DATE,
                                    YEAR = ObjDetail.NGAY_QUYET_TOAN.Value.Year,
                                    MONTH = ObjDetail.NGAY_QUYET_TOAN.Value.Month,
                                    UPDATE_BY = currentUser
                                });
                        projectTimeRepo.Queryable()
                                .Where(x => x.ID == lastProjectTimePeriod[lastProjectTimePeriod.Count() - 1].ID)
                                .Update(x => new T_PS_TIME
                                {
                                    START_DATE = ObjDetail.HAN_BAO_HANH ?? ObjDetail.FINISH_DATE,
                                    FINISH_DATE = ObjDetail.HAN_BAO_HANH ?? ObjDetail.FINISH_DATE,
                                    YEAR = ObjDetail.HAN_BAO_HANH.Value.Year,
                                    MONTH = ObjDetail.HAN_BAO_HANH.Value.Month,
                                    UPDATE_BY = currentUser
                                });
                    }
                    else
                    {
                        var timePeriodsChange = timePeriods
                            .Where(x => x.StartDate >= lastProjectTimePeriod[lastProjectTimePeriod.Count() - 3].START_DATE)
                            .ToList();
                        projectTimeRepo.Queryable()
                                .Where(x => x.ID == lastProjectTimePeriod[lastProjectTimePeriod.Count() - 3].ID)
                                .Update(x => new T_PS_TIME
                                {
                                    START_DATE = timePeriodsChange[0].StartDate,
                                    FINISH_DATE = timePeriodsChange[0].EndDate,
                                    YEAR = timePeriodsChange[0].EndDate.Year,
                                    MONTH = timePeriodsChange[0].EndDate.Month,
                                    UPDATE_BY = currentUser
                                });

                        for (int i = 1; i < timePeriodsChange.Count - 2; i++)
                        {
                            projectTimeRepo.Create(ConvertTimePeriodToPsTime(timePeriodsChange[i],
                                                                             lastProjectTimePeriod[lastProjectTimePeriod.Count() - 3].C_ORDER + i,
                                                                             currentUser));
                        }
                        projectTimeRepo.Queryable()
                                .Where(x => x.ID == lastProjectTimePeriod[lastProjectTimePeriod.Count() - 2].ID)
                                .Update(x => new T_PS_TIME
                                {
                                    START_DATE = ObjDetail.NGAY_QUYET_TOAN ?? ObjDetail.FINISH_DATE,
                                    FINISH_DATE = ObjDetail.NGAY_QUYET_TOAN ?? ObjDetail.FINISH_DATE,
                                    YEAR = ObjDetail.NGAY_QUYET_TOAN.Value.Year,
                                    MONTH = ObjDetail.NGAY_QUYET_TOAN.Value.Month,
                                    C_ORDER = x.C_ORDER + timePeriodsChange.Count - 3,
                                    UPDATE_BY = currentUser
                                });
                        projectTimeRepo.Queryable()
                                .Where(x => x.ID == lastProjectTimePeriod[lastProjectTimePeriod.Count() - 1].ID)
                                .Update(x => new T_PS_TIME
                                {
                                    START_DATE = ObjDetail.HAN_BAO_HANH ?? ObjDetail.FINISH_DATE,
                                    FINISH_DATE = ObjDetail.HAN_BAO_HANH ?? ObjDetail.FINISH_DATE,
                                    YEAR = ObjDetail.HAN_BAO_HANH.Value.Year,
                                    MONTH = ObjDetail.HAN_BAO_HANH.Value.Month,
                                    C_ORDER = x.C_ORDER + timePeriodsChange.Count - 3,
                                    UPDATE_BY = currentUser
                                });
                    }
                }

                projectStruct.FINISH_DATE = this.ObjDetail.FINISH_DATE;
                projectStruct.TEXT = this.ObjDetail.NAME;

                var checkPD = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == this.ObjDetail.ID && x.USER_NAME == this.ObjDetail.GIAM_DOC_DU_AN && x.PROJECT_ROLE_ID.Contains("PD")).FirstOrDefault();
                if (checkPD == null)
                {
                    UnitOfWork.GetSession().Save(new T_PS_RESOURCE()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = this.ObjDetail.ID,
                        USER_NAME = this.ObjDetail.GIAM_DOC_DU_AN,
                        PROJECT_ROLE_ID = "PD",
                        PROJECT_USER_TYPE_CODE = "N04",
                        FROM_DATE = DateTime.Now,
                        TO_DATE = this.ObjDetail.FINISH_DATE,
                        IS_SEND_MAIL = false
                    });
                }
                var checkPM = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == this.ObjDetail.ID && x.USER_NAME == this.ObjDetail.QUAN_TRI_DU_AN && x.PROJECT_ROLE_ID.Contains("PM")).FirstOrDefault();
                if (checkPM == null)
                {
                    UnitOfWork.GetSession().Save(new T_PS_RESOURCE()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = this.ObjDetail.ID,
                        USER_NAME = this.ObjDetail.QUAN_TRI_DU_AN,
                        PROJECT_ROLE_ID = "PM",
                        PROJECT_USER_TYPE_CODE = "N01",
                        FROM_DATE = DateTime.Now,
                        TO_DATE = this.ObjDetail.FINISH_DATE,
                        IS_SEND_MAIL = false
                    });
                }

                var checkSM = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == this.ObjDetail.ID && x.USER_NAME == this.ObjDetail.PROJECT_OWNER && x.PROJECT_ROLE_ID.Contains("SM")).FirstOrDefault();
                if (checkSM == null)
                {
                    UnitOfWork.GetSession().Save(new T_PS_RESOURCE()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = this.ObjDetail.ID,
                        USER_NAME = this.ObjDetail.PROJECT_OWNER,
                        PROJECT_ROLE_ID = "SM",
                        PROJECT_USER_TYPE_CODE = "N01",
                        FROM_DATE = DateTime.Now,
                        TO_DATE = this.ObjDetail.FINISH_DATE,
                        IS_SEND_MAIL = false
                    });
                }

                var checkQLHD = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == this.ObjDetail.ID && x.USER_NAME == this.ObjDetail.QUAN_LY_HOP_DONG && x.PROJECT_ROLE_ID.Contains("QLHD")).FirstOrDefault();
                if (checkQLHD == null)
                {
                    UnitOfWork.GetSession().Save(new T_PS_RESOURCE()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = this.ObjDetail.ID,
                        USER_NAME = this.ObjDetail.QUAN_LY_HOP_DONG,
                        PROJECT_ROLE_ID = "QLHD",
                        PROJECT_USER_TYPE_CODE = "N01",
                        FROM_DATE = DateTime.Now,
                        TO_DATE = this.ObjDetail.FINISH_DATE,
                        IS_SEND_MAIL = false
                    });
                }

                var checkPTCU = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == this.ObjDetail.ID && x.USER_NAME == this.ObjDetail.PHU_TRACH_CUNG_UNG && x.PROJECT_ROLE_ID.Contains("CU")).FirstOrDefault();
                if (checkPTCU == null)
                {
                    UnitOfWork.GetSession().Save(new T_PS_RESOURCE()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = this.ObjDetail.ID,
                        USER_NAME = this.ObjDetail.PHU_TRACH_CUNG_UNG,
                        PROJECT_ROLE_ID = "CU",
                        PROJECT_USER_TYPE_CODE = "N01",
                        FROM_DATE = DateTime.Now,
                        TO_DATE = this.ObjDetail.FINISH_DATE,
                        IS_SEND_MAIL = false
                    });
                }

                UnitOfWork.GetSession().Query<T_PS_CONTRACT>()
                    .Where(x => x.PROJECT_ID == ObjDetail.ID && x.CONTRACT_TYPE.Contains("KD"))
                    .Update(x => new T_PS_CONTRACT
                    {
                        CUSTOMER_CODE = ObjDetail.CUSTOMER_CODE
                    });

                UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>()
                    .Where(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                    .Delete();
                UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                {
                    ID = Guid.NewGuid(),
                    PROJECT_STRUCT_ID = projectStruct.ID,
                    PROJECT_ID = this.ObjDetail.ID,
                    ACTION = "U"
                });

                UnitOfWork.GetSession().Update(projectStruct);
                UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.ID, currentUser, "Thông tin dự án");
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }

        }
        internal void CloseProject()
        {
            try
            {
                Get(ObjDetail.ID);

                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                this.ObjDetail.STATUS = ProjectStatus.DONG_DU_AN.GetValue();
                this.CurrentRepository.Update(this.ObjDetail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void DoneProject()
        {
            try
            {
                Get(ObjDetail.ID);

                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                this.ObjDetail.STATUS = ProjectStatus.HOAN_THANH.GetValue();
                this.CurrentRepository.Update(this.ObjDetail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
        internal void StartProject()
        {
            try
            {
                Get(ObjDetail.ID);
                if (ObjDetail.START_DATE == null || ObjDetail.FINISH_DATE == null)
                {
                    State = false;
                    ErrorMessage = "Thời gian bắt đầu và kết thúc dự án chưa được thiết lập.";
                    return;
                }

                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                this.ObjDetail.STATUS = ProjectStatus.LAP_KE_HOACH.GetValue();

                IList<TimePeriod> timePeriods = new List<TimePeriod>();
                if (ObjDetail.TIME_TYPE == ProjectTimeTypeEnum.WEEK.GetValue())
                {
                    timePeriods = SMOUtilities.GenerateTimeWeek(ObjDetail.START_DATE, ObjDetail.FINISH_DATE, ObjDetail.NGAY_QUYET_TOAN, ObjDetail.HAN_BAO_HANH);
                }
                else if (ObjDetail.TIME_TYPE == ProjectTimeTypeEnum.MONTH.GetValue())
                {
                    timePeriods = SMOUtilities.GenerateTimeMonth(ObjDetail.START_DATE, ObjDetail.FINISH_DATE, ObjDetail.NGAY_QUYET_TOAN, ObjDetail.HAN_BAO_HANH);
                }

                UnitOfWork.Repository<ProjectTimeRepo>()
                    .Create(lstObj: (from period in timePeriods
                                     let index = timePeriods.IndexOf(period)
                                     select ConvertTimePeriodToPsTime(period, index, currentUser))
                                                                        .ToList());
                this.CurrentRepository.Update(this.ObjDetail);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        private T_PS_TIME ConvertTimePeriodToPsTime(TimePeriod period, int index, string currentUser)
        {
            return new T_PS_TIME
            {
                ID = Guid.NewGuid(),
                PROJECT_ID = ObjDetail.ID,
                START_DATE = period.StartDate,
                FINISH_DATE = period.EndDate,
                MONTH = period.StartDate.Month,
                YEAR = period.StartDate.Year,
                TIME_TYPE = ObjDetail.TIME_TYPE,
                C_ORDER = index,
                CREATE_BY = currentUser
            };
        }

        internal IList<ProjectStructureBoqModel> GetProjectStructureBoq(Guid id)
        {
            var projectStructures = GetStructures(id, new List<ProjectEnum>() { ProjectEnum.BOQ, ProjectEnum.PROJECT });
            var contractDetails = GetContractDetails(id);
            var projectStructure = projectStructures.FirstOrDefault(x => x.TYPE == ProjectEnum.PROJECT.ToString());
            return (from projectStruct in projectStructures
                    where projectStruct.TYPE == ProjectEnum.BOQ.ToString()
                    let contractDetail = contractDetails.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                    select new ProjectStructureBoqModel
                    {
                        ProjectStructureId = projectStructure?.ID,
                        Parent = projectStruct.PARENT_ID,
                        ProjectId = id,
                        ProjectStructId = projectStruct.ID,
                        BoqId = projectStruct.BOQ_ID.Value,
                        TEXT = projectStruct.TEXT,
                        TYPE = projectStruct.TYPE,
                        UNIT_NAME = contractDetail?.Unit?.NAME,
                        CONTRACT_VALUE = contractDetail?.VOLUME,
                        START_DATE = projectStruct.START_DATE,
                        FINISH_DATE = projectStruct.FINISH_DATE,
                    }).ToList();
        }

        internal IList<ProjectStructureCostModel> GetProjectStructureCost(Guid id)
        {
            var projectStructures = GetStructures(id, new List<ProjectEnum>() { ProjectEnum.PROJECT, ProjectEnum.WBS, ProjectEnum.ACTIVITY });
            var contractDetails = GetContractDetails(id);
            return (from projectStruct in projectStructures
                    let contractDetail = contractDetails.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                    select new ProjectStructureCostModel
                    {
                        Id = projectStruct.ID,
                        Parent = projectStruct.PARENT_ID,
                        ProjectId = id,
                        ProjectStructId = projectStruct.ID,
                        TEXT = projectStruct.TEXT,
                        TYPE = projectStruct.TYPE,
                        UNIT_NAME = contractDetail?.Unit?.NAME,
                        CONTRACT_VALUE = contractDetail?.VOLUME,
                        START_DATE = projectStruct.START_DATE,
                        FINISH_DATE = projectStruct.FINISH_DATE,
                        ReferenceBoqId = projectStruct.Wbs?.BOQ_REFRENCE_ID
                    }).ToList();
        }

        internal void UpdateInformationStructureCostModel(IList<UpdateInformationStructureCostModel> data)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;

                var projectStructureIds = from d in data
                                          select d.ProjectStructId;
                var projectStructures = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                    .Where(x => projectStructureIds.Contains(x.ID))
                    .ToList();
                var wbsRepo = UnitOfWork.Repository<WbsRepo>();
                foreach (var projectStructure in projectStructures)
                {
                    var structureCost = data.FirstOrDefault(x => x.ProjectStructId == projectStructure.ID);
                    if (structureCost != null && projectStructure.Wbs != null)
                    {
                        projectStructure.Wbs.BOQ_REFRENCE_ID = structureCost.ReferenceBoqId;
                        projectStructure.Wbs.UPDATE_BY = currentUser;
                        wbsRepo.Update(projectStructure.Wbs);
                    }
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

        internal void UpdateProjectPlanStatus(UpdateProjectPlanStatusModel model, ProjectPlanStatus statusUpdate)
        {
            try
            {
                var project = CurrentRepository.Get(model.ProjectId);
                CurrentRepository.Detach(project);

                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                project.UPDATE_BY = currentUser;
                var previousStatus = string.Empty;
                switch (model.Type)
                {
                    case ProjectPartnerType.VENDOR:
                        switch (model.SubType)
                        {
                            case ProjectPlanType.COST:
                                previousStatus = project.STATUS_VENDOR_PLAN_COST;
                                project.STATUS_VENDOR_PLAN_COST = statusUpdate.GetValue();
                                break;
                            case ProjectPlanType.PROGRESS:
                                previousStatus = project.STATUS_VENDOR_PLAN_PROGRESS;
                                project.STATUS_VENDOR_PLAN_PROGRESS = statusUpdate.GetValue();
                                break;
                            case ProjectPlanType.QUANTITY:
                                previousStatus = project.STATUS_VENDOR_PLAN_QUANTITY;
                                project.STATUS_VENDOR_PLAN_QUANTITY = statusUpdate.GetValue();
                                break;
                            default:
                                break;
                        }
                        break;
                    case ProjectPartnerType.SL_DT:
                        previousStatus = project.STATUS_SL_DT;
                        project.STATUS_SL_DT = statusUpdate.GetValue();
                        break;
                    case ProjectPartnerType.CUSTOMER:
                        switch (model.SubType)
                        {
                            case ProjectPlanType.COST:
                                previousStatus = project.STATUS_CUSTOMER_PLAN_COST;
                                project.STATUS_CUSTOMER_PLAN_COST = statusUpdate.GetValue();
                                break;
                            case ProjectPlanType.PROGRESS:
                                previousStatus = project.STATUS_CUSTOMER_PLAN_PROGRESS;
                                project.STATUS_CUSTOMER_PLAN_PROGRESS = statusUpdate.GetValue();
                                break;
                            case ProjectPlanType.QUANTITY:
                                previousStatus = project.STATUS_CUSTOMER_PLAN_QUANTITY;
                                project.STATUS_CUSTOMER_PLAN_QUANTITY = statusUpdate.GetValue();
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }

                UnitOfWork.Repository<PlanProgressHistoryRepo>()
                    .Create(new T_PS_PLAN_PROGRESS_HISTORY
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = project.ID,
                        PLAN_TYPE = model.SubType.ToString(),
                        ACTOR = currentUser,
                        CREATE_BY = currentUser,
                        ACTION = statusUpdate.GetValue(),
                        DES_STATUS = statusUpdate.GetValue(),
                        PRE_STATUS = previousStatus,
                        NOTE = model.Note,
                        PARTNER_TYPE = model.Type.ToString(),
                    });
                CurrentRepository.Update(project);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void SaveComment(Guid projectId, string comment)
        {
            UnitOfWork.Clear();
            UnitOfWork.BeginTransaction();
            UnitOfWork.Repository<CommentRepo>()
                    .Create(new T_PS_COMMENT
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = projectId,
                        MESSENGER = comment,
                        USER_NAME = ProfileUtilities.User?.USER_NAME,
                        IS_FILE = "N",
                    });
            UnitOfWork.Commit();

            var project = UnitOfWork.Repository<ProjectRepo>().Queryable().Where(x => x.ID == projectId).FirstOrDefault();

            string str = comment.Replace(" ", ",");
            string[] arrayString = new string[] { "" };
            arrayString = str.Split(',');

            foreach (var item in arrayString)
            {
                if (item.Contains("@"))
                {
                    string username = item.Replace("@", "");
                    var user = UnitOfWork.Repository<UserRepo>().Queryable().Where(x => x.USER_NAME == username).FirstOrDefault();
                    if (user != null)
                    {
                        var senderEmail = new MailAddress("pms-fecon@fecon.com.vn", "Hệ thống PMS");
                        var receiverEmail = new MailAddress(user.EMAIL, user.FULL_NAME);
                        var password = "P2ss@2022";
                        var sub = "Thông báo!";
                        var body = $"<p>Dear Anh/ chị,</p>\r\n\r\n<p>Anh/Chị đ&atilde; được nhắc đến trong một b&igrave;nh luận tr&ecirc;n hệ thống PMS tại dự &aacute;n {project.CODE} : {project.NAME}. Vui l&ograve;ng truy cập tại đ&acirc;y https://pmstest.fecon.com.vn/ để xem.</p>\r\n\r\n<table border=\"1\" cellpadding=\"1\" cellspacing=\"1\" style=\"width:500px\">\r\n\t<tbody>\r\n\t\t<tr>\r\n\t\t\t<td colspan=\"2\">Trao đổi công việc trên Dashboard</td>\r\n\t\t</tr>\r\n\t\t<tr>\r\n\t\t\t<td>Dự &aacute;n</td>\r\n\t\t\t<td>{project.CODE} - {project.NAME}</td>\r\n\t\t</tr>\r\n\t\t<tr>\r\n\t\t\t<td>Người y&ecirc;u cầu</td>\r\n\t\t\t<td>{ProfileUtilities.User?.FULL_NAME}</td>\r\n\t\t</tr>\r\n\t\t<tr>\r\n\t\t\t<td>Nội dung</td>\r\n\t\t\t<td>{comment}</td>\r\n\t\t</tr>\r\n\t\t<tr>\r\n\t\t\t<td>Ng&agrave;y giờ</td>\r\n\t\t\t<td>&nbsp;{DateTime.Now}</td>\r\n\t\t</tr>\r\n\t</tbody>\r\n</table>\r\n\r\n<p>Tr&acirc;n trọng!</p>\r\n\r\n<p>Đ&acirc;y l&agrave; email được gửi tự động từ phần mềm PMS, xin vui l&ograve;ng kh&ocirc;ng trả lời lại email n&agrave;y.</p>\r\n";
                        var smtp = new SmtpClient
                        {
                            Host = "mail.fecon.com.vn",
                            Port = 587,
                            EnableSsl = false,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = sub,
                            Body = body,
                            IsBodyHtml = true,
                        })
                        {
                            smtp.Send(mess);
                        }
                    }
                }
            }
        }

        internal void SaveFileComment(Guid projectId, string fileName, string filePath, string mimeType)
        {
            UnitOfWork.Clear();
            UnitOfWork.BeginTransaction();
            UnitOfWork.Repository<CommentRepo>()
                    .Create(new T_PS_COMMENT
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = projectId,
                        MESSENGER = fileName,
                        USER_NAME = ProfileUtilities.User?.USER_NAME,
                        IS_FILE = "Y",
                        MIME_TYPE = mimeType,
                        PATH_FILE = filePath,
                    });
            UnitOfWork.Commit();
        }
        internal IList<T_PS_COMMENT> GetAllComment(Guid projectId)
        {
            return UnitOfWork.Repository<CommentRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).OrderBy(x => x.CREATE_DATE).ToList();
        }
        internal IList<T_PS_RESOURCE> GetResourceProject(Guid projectId)
        {
            return UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).ToList();
        }

        internal ProjectStructureProgressStatus GetStatusFromAction(ProjectStructureProgressAction action)
        {
            switch (action)
            {
                case ProjectStructureProgressAction.TAO_MOI:
                    return ProjectStructureProgressStatus.KHOI_TAO;
                case ProjectStructureProgressAction.GUI:
                    return ProjectStructureProgressStatus.CHO_PHE_DUYET;
                case ProjectStructureProgressAction.PHE_DUYET:
                    return ProjectStructureProgressStatus.PHE_DUYET;
                case ProjectStructureProgressAction.TU_CHOI:
                    return ProjectStructureProgressStatus.TU_CHOI;
                case ProjectStructureProgressAction.HUY_PHE_DUYET:
                    return ProjectStructureProgressStatus.CHO_PHE_DUYET;
                default:
                    return ProjectStructureProgressStatus.KHOI_TAO;
            }
        }
        internal void UpdateProjectStructureStatus(UpdateProjectStructureStatusModel model)
        {
            try
            {
                var project = CurrentRepository.Get(model.ProjectId);
                CurrentRepository.Detach(project);

                var destinationStatus = GetStatusFromAction(model.Action).GetValue();
                //if (destinationStatus == ProjectStructureProgressStatus.PHE_DUYET.GetValue())
                //{
                //    var lstCode = new List<string>() { "C.0397", "C.0390", "C.0382", "C.0383", "C.0387", "C.0380", "C.0389", "C.0395", "C.0384", "C.0381" };
                //    if (!lstCode.Contains(project.CODE))
                //    {
                //        this.PostDataToSAP(model.ProjectId);
                //        if (!this.State)
                //        {
                //            this.ErrorMessage = "Đẩy thông tin lên SAP không thành công." + this.ErrorMessage;
                //            return;
                //        }
                //    }
                //}

                project = CurrentRepository.Get(model.ProjectId);
                CurrentRepository.Detach(project);

                UnitOfWork.Clear();
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.Repository<ProgressHistoryRepo>().Create(new T_PS_PROGRESS_HISTORY
                {
                    ID = Guid.NewGuid(),
                    ACTOR = currentUser,
                    CREATE_BY = currentUser,
                    PROJECT_ID = model.ProjectId,
                    ACTION = model.Action.GetValue(),
                    PRE_STATUS = project.STATUS_STRUCT_PLAN ?? ProjectStructureProgressStatus.KHOI_TAO.GetValue(),
                    DES_STATUS = destinationStatus,
                    NOTE = model.Note,
                });
                if (destinationStatus == ProjectStructureProgressStatus.PHE_DUYET.GetValue())
                {
                    project.STATUS = ProjectStatus.BAT_DAU.GetValue();
                }
                project.UPDATE_BY = currentUser;
                project.STATUS_STRUCT_PLAN = destinationStatus;
                CurrentRepository.Update(project);

                UnitOfWork.Commit();
                this.CreateNotiEmail(project, model.Note);
                this.CreateNotify(project);
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        private IList<T_PS_CONTRACT_DETAIL> GetContractDetails(Guid id)
        {
            return UnitOfWork.Repository<ContractDetailRepo>().Queryable()
                    .Where(x => x.Struct.PROJECT_ID == id)
                    .ToList();
        }

        internal IList<T_PS_PROJECT_STRUCT> GetStructures(Guid projectId, IList<ProjectEnum> projectEnums)
        {
            var projectTypesStr = from projectEnum in projectEnums
                                  select projectEnum.ToString();
            return UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId && projectTypesStr.Contains(x.TYPE))
                .OrderBy(x => x.C_ORDER).ThenBy(x => x.CREATE_DATE)
                .ToList();
        }

        public void CreateNotiEmail(T_PS_PROJECT project, string note)
        {
            try
            {
                List<string> lstEmail = new List<string>();
                var lstProjectUser = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID).ToList();
                var lstUserApprove = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("PM")).ToList();
                var currentUser = lstProjectUser.FirstOrDefault(x => x.USER_NAME == ProfileUtilities.User.USER_NAME);

                string LOAI_CONG_VIEC = "{LOAI_CONG_VIEC}";
                string MA = "{MA}";
                string TEN = "{TEN}";
                string Y_KIEN = "{Y_KIEN}";
                string USER_NAME_NGUOI_DE_XUAT = "{USER_NAME_NGUOI_DE_XUAT}";
                string HO_TEN_NGUOI_DE_XUAT = "{HO_TEN_NGUOI_DE_XUAT}";
                string USER_NAME_NGUOI_PHE_DUYET = "{USER_NAME_NGUOI_PHE_DUYET}";
                string HO_TEN_NGUOI_PHE_DUYET = "{HO_TEN_NGUOI_PHE_DUYET}";
                string TRANG_THAI = "{TRANG_THAI}";
                string LINK_CHI_TIET = "{LINK_CHI_TIET}";
                string VAI_TRO_TAI_DU_AN = "{VAI_TRO_TAI_DU_AN}";

                var serviceTemplate = new ConfigTemplateNotifyService();
                serviceTemplate.GetAll();
                if (serviceTemplate.ObjList.Count() == 0)
                {
                    return;
                }

                var serviceConfig = new SystemConfigService();
                serviceConfig.GetConfig();

                var template = serviceTemplate.ObjList.FirstOrDefault();

                var serviceEmail = new EmailNotifyService();
                var url = HttpContext.Current.Request.Url.Host;

                var contentSubjectXuLy = template.CONG_VIEC_XU_LY_SUBJECT
                        .Replace(LOAI_CONG_VIEC, "Thông tin dự án")
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME);


                var contentBodyXuLy = template.CONG_VIEC_XU_LY_BODY
                        .Replace(LOAI_CONG_VIEC, "Thông tin dự án")
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME)
                        .Replace(TRANG_THAI, project.STATUS_STRUCT_PLAN.GetEnum<ProjectStructureProgressStatus>().GetName())
                        .Replace(LINK_CHI_TIET, url + "/Home/OpenProject?id=" + project.ID)
                        .Replace(Y_KIEN, note)
                        .Replace(HO_TEN_NGUOI_DE_XUAT, ProfileUtilities.User.FULL_NAME);


                var contentSubjectHoanThanh = template.CONG_VIEC_HOAN_THANH_SUBJECT
                        .Replace(LOAI_CONG_VIEC, "Thông tin dự án")
                        .Replace(TRANG_THAI, project.STATUS_STRUCT_PLAN.GetEnum<ProjectStructureProgressStatus>().GetName())
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME);


                var contentBodyHoanThanh = template.CONG_VIEC_HOAN_THANH_BODY
                        .Replace(LOAI_CONG_VIEC, "Thông tin dự án")
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME)
                        .Replace(TRANG_THAI, project.STATUS_STRUCT_PLAN.GetEnum<ProjectStructureProgressStatus>().GetName())
                        .Replace(LINK_CHI_TIET, url + "/Home/OpenProject?id=" + project.ID)
                        .Replace(Y_KIEN, note)
                        .Replace(HO_TEN_NGUOI_PHE_DUYET, ProfileUtilities.User.FULL_NAME);

                var contentSubjectChinhSua = template.PHE_DUYET_CHINH_SUA_SUBJECT
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME);


                var contentBodyChinhSua = template.PHE_DUYET_CHINH_SUA_BODY
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME)
                        .Replace(LINK_CHI_TIET, url + "/Home/OpenProject?id=" + project.ID)
                        .Replace(TRANG_THAI, project.STATUS_STRUCT_PLAN.GetEnum<ProjectStructureProgressStatus>().GetName())
                        .Replace(Y_KIEN, note)
                        .Replace(HO_TEN_NGUOI_DE_XUAT, ProfileUtilities.User.FULL_NAME);

                var contentSubjectNhanSu = template.NHAN_SU_THAM_GIA_SUBJECT
                        .Replace(LOAI_CONG_VIEC, "Thông tin dự án")
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME);


                var contentBodyNhanSu = template.NHAN_SU_THAM_GIA_BODY
                        .Replace(MA, project.CODE)
                        .Replace(TEN, project.NAME)
                        .Replace(LINK_CHI_TIET, url + "/Home/OpenProject?id=" + project.ID);

                var history = UnitOfWork.GetSession().Query<T_PS_PROGRESS_HISTORY>().Where(x => x.PROJECT_ID == project.ID).OrderBy(x => x.CREATE_DATE).ToList();

                UnitOfWork.BeginTransaction();

                if (project.STATUS_STRUCT_PLAN == ProjectStructureProgressStatus.CHO_PHE_DUYET.GetValue())
                {
                    if (history.Count(x => x.ACTION == ProjectStructureProgressAction.PHE_DUYET.GetValue()) == 0)
                    {
                        foreach (var user in lstUserApprove)
                        {
                            if (user.FROM_DATE.HasValue && user.TO_DATE.HasValue)
                            {
                                int result1 = DateTime.Compare(DateTime.Now, user.FROM_DATE.Value);
                                int result2 = DateTime.Compare(DateTime.Now, user.TO_DATE.Value);
                                if (result1 >= 0 && result2 <= 0)
                                {
                                    contentBodyXuLy = contentBodyXuLy.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID);
                                    UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                                    {
                                        PKID = Guid.NewGuid().ToString(),
                                        EMAIL = user.User.EMAIL,
                                        SUBJECT = contentSubjectXuLy,
                                        CONTENTS = contentBodyXuLy
                                    });
                                }
                            }

                        }
                    }
                    else
                    {
                        var thongTinThayDoi = "";
                        var lanPheDuyetCuoiCung = history.Last(x => x.ACTION == ProjectStructureProgressAction.PHE_DUYET.GetValue());
                        var lanThayDoiThongTin = history.Where(x => x.CREATE_DATE > lanPheDuyetCuoiCung.CREATE_DATE).ToList();
                        thongTinThayDoi = String.Join(",", lanThayDoiThongTin.Select(x => x.TAB_NAME).Distinct().ToList());

                        foreach (var user in lstUserApprove)
                        {
                            if (user.FROM_DATE.HasValue && user.TO_DATE.HasValue)
                            {
                                int result1 = DateTime.Compare(DateTime.Now, user.FROM_DATE.Value);
                                int result2 = DateTime.Compare(DateTime.Now, user.TO_DATE.Value);
                                if (result1 >= 0 && result2 <= 0)
                                {
                                    contentBodyChinhSua = contentBodyChinhSua.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                                    .Replace("{NOI_DUNG_DIEU_CHINH}", thongTinThayDoi);
                                    UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                                    {
                                        PKID = Guid.NewGuid().ToString(),
                                        EMAIL = user.User.EMAIL,
                                        SUBJECT = contentSubjectChinhSua,
                                        CONTENTS = contentBodyChinhSua
                                    });
                                }
                            }
                        }
                    }
                }

                if (project.STATUS_STRUCT_PLAN == ProjectStructureProgressStatus.PHE_DUYET.GetValue())
                {
                    contentBodyHoanThanh = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, "");
                    UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        EMAIL = project.USER_CREATE.EMAIL,
                        SUBJECT = contentSubjectHoanThanh,
                        CONTENTS = contentBodyHoanThanh
                    });

                    if (history.Count(x => x.ACTION == ProjectStructureProgressAction.PHE_DUYET.GetValue()) == 0)
                    {
                        foreach (var user in lstProjectUser)
                        {
                            UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                EMAIL = user.User.EMAIL,
                                SUBJECT = contentSubjectNhanSu,
                                CONTENTS = contentBodyNhanSu
                            });

                            UnitOfWork.GetSession().Query<T_PS_RESOURCE>().Where(x => x.ID == user.ID).Update(x => new T_PS_RESOURCE()
                            {
                                IS_SEND_MAIL = true
                            });
                        }
                    }
                }

                if (project.STATUS_STRUCT_PLAN == ProjectStructureProgressStatus.TU_CHOI.GetValue())
                {
                    contentBodyHoanThanh = contentBodyHoanThanh.Replace(VAI_TRO_TAI_DU_AN, "");
                    UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                    {
                        PKID = Guid.NewGuid().ToString(),
                        EMAIL = project.USER_CREATE.EMAIL,
                        SUBJECT = contentSubjectHoanThanh,
                        CONTENTS = contentBodyHoanThanh
                    });
                }

                UnitOfWork.Commit();
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();
            }
        }

        public void CreateNotify(T_PS_PROJECT project)
        {
            try
            {
                var lstProjectUser = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID).ToList();
                var lstUserApprove = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("PM")).ToList();
                var history = UnitOfWork.GetSession().Query<T_PS_PROGRESS_HISTORY>().Where(x => x.PROJECT_ID == project.ID).OrderBy(x => x.CREATE_DATE).ToList();

                var lstUserNotify = new List<string>();
                string strTemplate = @"
                            <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                                <div class='icon-circle {2}'>
                                    <i class='material-icons'>{3}</i>
                                </div>
                                <div class='menu-info'>
                                    <span>Dự án [{4} - {5}] {6}!</span>
                                    <p>
                                        <i class='material-icons'>access_time</i> {7}
                                    </p>
                                </div>
                            </a>
                        ";

                UnitOfWork.BeginTransaction();

                if (project.STATUS_STRUCT_PLAN == ProjectStructureProgressStatus.CHO_PHE_DUYET.GetValue())
                {
                    lstUserNotify.AddRange(lstUserApprove.Select(x => x.USER_NAME).ToList());
                    foreach (var user in lstUserApprove)
                    {
                        var newId = Guid.NewGuid().ToString();
                        string strContent = string.Format(strTemplate,
                                newId,
                                $"/PS/Project/Edit?id={project.ID}",
                                "",
                                "",
                                project.CODE,
                                project.NAME,
                                "chờ phê duyệt",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                        string strRawContent = $"Dự án {project.CODE} đang chờ phê duyệt!";
                        UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                        {
                            PKID = newId,
                            CONTENTS = strContent,
                            RAW_CONTENTS = strRawContent,
                            USER_NAME = user.USER_NAME
                        });
                    }
                }

                if (project.STATUS_STRUCT_PLAN == ProjectStructureProgressStatus.PHE_DUYET.GetValue())
                {
                    lstUserNotify.Add(project.USER_CREATE.USER_NAME);
                    var newId = Guid.NewGuid().ToString();
                    string strContent = string.Format(strTemplate,
                            newId,
                            $"/PS/Project/Edit?id={project.ID}",
                            "",
                            "",
                            project.CODE,
                            project.NAME,
                            "đã được phê duyệt",
                            DateTime.Now.ToString(Global.DateTimeToStringFormat));

                    string strRawContent = $"Dự án {project.CODE} đã được phê duyệt!";

                    UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        RAW_CONTENTS = strRawContent,
                        USER_NAME = project.USER_CREATE.USER_NAME
                    });

                    if (history.Count(x => x.ACTION == ProjectStructureProgressAction.PHE_DUYET.GetValue()) == 0)
                    {
                        lstUserNotify.AddRange(lstProjectUser.Select(x => x.USER_NAME).ToList());
                        foreach (var user in lstProjectUser)
                        {
                            newId = Guid.NewGuid().ToString();
                            strContent = string.Format(strTemplate,
                                newId,
                                $"/PS/Project/Edit?id={project.ID}",
                                "",
                                "",
                                project.CODE,
                                project.NAME,
                                ": Bạn đã được thêm vào dự án",
                                DateTime.Now.ToString(Global.DateTimeToStringFormat));

                            strRawContent = $"Bạn được thêm vào Dự án {project.CODE}!";
                            UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                            {
                                PKID = newId,
                                CONTENTS = strContent,
                                RAW_CONTENTS = strRawContent,
                                USER_NAME = user.USER_NAME
                            });
                        }
                    }
                }

                if (project.STATUS_STRUCT_PLAN == ProjectStructureProgressStatus.TU_CHOI.GetValue())
                {
                    lstUserNotify.Add(project.USER_CREATE.USER_NAME);
                    var newId = Guid.NewGuid().ToString();
                    string strContent = string.Format(strTemplate,
                            newId,
                            $"/PS/Project/Edit?id={project.ID}",
                            "",
                            "",
                            project.CODE,
                            project.NAME,
                            "đã bị từ chối",
                            DateTime.Now.ToString(Global.DateTimeToStringFormat));

                    string strRawContent = $"Dự án {project.CODE} đã bị từ chối!";

                    UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                    {
                        PKID = newId,
                        CONTENTS = strContent,
                        RAW_CONTENTS = strRawContent,
                        USER_NAME = project.USER_CREATE.USER_NAME
                    });
                }

                UnitOfWork.Commit();

                SMOUtilities.SendNotify(lstUserNotify);
            }
            catch (Exception)
            {
                UnitOfWork.Rollback();
            }

        }

        internal IEnumerable<T_PS_PROGRESS_HISTORY> GetProgressHistory(Guid projectId)
        {
            return UnitOfWork.Repository<ProgressHistoryRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId)
                .OrderByDescending(x => x.CREATE_DATE)
                .ToList();
        }

        internal IEnumerable<T_PS_VOLUME_PROGRESS_HISTORY> GetVolumeProgressHistory(Guid resourceId)
        {
            return UnitOfWork.Repository<VolumeProgressHistoryRepo>().Queryable()
                .Where(x => x.RESOURCE_ID == resourceId)
                .OrderByDescending(x => x.CREATE_DATE)
                .ToList();
        }

        internal IEnumerable<T_MD_VENDOR> GetVendors()
        {
            return UnitOfWork.Repository<Repository.Implement.MD.VendorRepo>().GetAllOrdered(nameof(T_MD_VENDOR.NAME));
        }
        internal IEnumerable<T_MD_CUSTOMER> GetCustomers()
        {
            return UnitOfWork.Repository<Repository.Implement.MD.CustomerRepo>().GetAllOrdered(nameof(T_MD_CUSTOMER.NAME));
        }


        //Lấy dữ liệu lên dashboard
        public decimal? GetCA(Guid projectId)
        {
            try
            {
                var data = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.CONTRACT_TYPE.Contains("KD"));
                if (data.FirstOrDefault() == null)
                {
                    return 0;
                }
                else
                {
                    return data.Sum(x => x.CONTRACT_VALUE);
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }


        }
        public decimal? GetBAC(Guid projectId)
        {
            try
            {
                var data = UnitOfWork.Repository<Repository.Implement.PS.ProjectRepo>().Queryable().Where(x => x.ID == projectId);
                if (data.FirstOrDefault() == null)
                {
                    return 0;
                }
                else
                {
                    return data.FirstOrDefault().TOTAL_COST;
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }
        public decimal? GetWP(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                DateTime endMonth = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));
                var valuePlanCost = (from x in UnitOfWork.Repository<Repository.Implement.PS.PlanCostRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && (x.TimePeriod.FINISH_DATE <= toDate || x.TimePeriod.FINISH_DATE == endMonth) /*fromDate <= x.TimePeriod.FINISH_DATE && toDate >= x.TimePeriod.FINISH_DATE*/ && x.IS_CUSTOMER == true)
                                     group x by x.PROJECT_STRUCT_ID into y
                                     select new
                                     {
                                         projectStructId = y.Key,
                                         value = y.Sum(x => x.VALUE),
                                     }).ToList();
                var listProjectStruct = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).ToList();
                var WP = (from x in listProjectStruct
                          join y in valuePlanCost on x.ID equals y.projectStructId
                          select new { projectId = x.PROJECT_ID, price = x.PRICE, value = y.value }).ToList();
                return WP.Sum(x => x.price * x.value);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }
        public decimal? GetWD(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var headerId = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == true && x.STATUS == "05" && x.TO_DATE <= toDate).ToList();
                var detailWorkVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkDetailRepo>().GetAll().ToList();
                var allDetailByHeaderId = (from x in detailWorkVolume
                                           join y in headerId on x.HEADER_ID equals y.ID
                                           select new { value = x.VALUE, price = x.PRICE }).ToList();
                return allDetailByHeaderId.Sum(x => x.price * x.value);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }
        public decimal? GetACW(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var headerId = UnitOfWork.Repository<Repository.Implement.PS.VolumeAcceptRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == true && x.STATUS == "05" && x.TO_DATE <= toDate /*fromDate <= x.TO_DATE && toDate >= x.TO_DATE*/).ToList();
                var detailAcceptVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeAcceptDetailRepo>().GetAll().ToList();
                var allDetailByHeaderId = (from x in detailAcceptVolume
                                           join y in headerId on x.HEADER_ID equals y.ID
                                           select new { value = x.VALUE, price = x.PRICE }).ToList();
                return allDetailByHeaderId.Sum(x => x.price * x.value);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }
        public decimal? GetPE(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                DateTime endMonth = new DateTime(toDate.Year, toDate.Month, DateTime.DaysInMonth(toDate.Year, toDate.Month));
                var valuePlanCost = (from x in UnitOfWork.Repository<Repository.Implement.PS.PlanCostRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && (x.TimePeriod.FINISH_DATE <= toDate || x.TimePeriod.FINISH_DATE == endMonth) /*fromDate <= x.TimePeriod.FINISH_DATE && toDate >= x.TimePeriod.FINISH_DATE*/ && x.IS_CUSTOMER == false)
                                     group x by x.PROJECT_STRUCT_ID into y
                                     select new
                                     {
                                         projectStructId = y.Key,
                                         value = y.Sum(x => x.VALUE),
                                     }).ToList();
                var listProjectStruct = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).ToList();
                var PE = (from x in listProjectStruct
                          join y in valuePlanCost on x.ID equals y.projectStructId
                          select new { projectId = x.PROJECT_ID, price = x.PRICE, value = y.value }).ToList();
                return PE.Sum(x => x.price * x.value);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }
        public decimal? GetAC(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var headerId = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == false && x.STATUS == "05" && x.TO_DATE <= toDate).ToList();
                var detailWorkVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkDetailRepo>().GetAll().ToList();
                var allDetailByHeaderId = (from x in detailWorkVolume
                                           join y in headerId on x.HEADER_ID equals y.ID
                                           select new { value = x.VALUE, price = x.PRICE }).ToList();
                return allDetailByHeaderId.Sum(x => x.price * x.value);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }
        public decimal? GetNT(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var headerId = UnitOfWork.Repository<Repository.Implement.PS.VolumeAcceptRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == false && x.STATUS == "05" && fromDate <= x.TO_DATE && toDate >= x.TO_DATE).ToList();
                var detailAcceptVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeAcceptDetailRepo>().GetAll().ToList();
                var allDetailByHeaderId = (from x in detailAcceptVolume
                                           join y in headerId on x.HEADER_ID equals y.ID
                                           select new { value = x.VALUE, price = x.PRICE }).ToList();
                return allDetailByHeaderId.Sum(x => x.price * x.value);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }
        public decimal? GetDoanhThuDuKien(Guid projectId)
        {
            try
            {
                var DoanhThuDuKien = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.TYPE == "BOQ").ToList();

                return DoanhThuDuKien.Sum(x => x.PLAN_VOLUME * x.PRICE);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }

        public Array GetDataDashboardBOQ(Guid projectId)
        {
            try
            {
                var allTimeInProject = UnitOfWork.Repository<Repository.Implement.PS.ProjectTimeRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).OrderBy(x => x.C_ORDER).ToList();

                //Lấy giá trị kế hoạch theo thời gian
                var planCostBOQ = UnitOfWork.Repository<Repository.Implement.PS.PlanCostRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == true).ToList();
                var lstProjectStruct = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId);
                var allPlanCostByTimeId = (from x in planCostBOQ
                                           join y in lstProjectStruct on x.PROJECT_STRUCT_ID equals y.ID
                                           select new { timeId = x.TIME_PERIOD_ID, value = x.VALUE, price = y.PRICE }).ToList();
                var planCost = (from x in allPlanCostByTimeId
                                group x by x.timeId into y
                                select new
                                {
                                    projectTimeId = y.Key,
                                    total = y.Sum(x => x.value * x.price),
                                }).ToList();

                //Lấy giá trị thực hiện theo thời gian
                var headerIdWork = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == true && x.STATUS == "05").ToList();
                var detailWorkVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkDetailRepo>().GetAll().ToList();
                var allDetailByHeaderIdWork = (from x in detailWorkVolume
                                               join y in headerIdWork on x.HEADER_ID equals y.ID
                                               select new { timeId = y.TIME_PERIOD_ID, value = x.VALUE, price = x.PRICE }).ToList();
                var volumeWork = (from x in allDetailByHeaderIdWork
                                  group x by x.timeId into y
                                  select new
                                  {
                                      projectTimeId = y.Key,
                                      total = y.Sum(x => x.value * x.price),
                                  }).ToList();

                //Lấy giá trị nghiệm thu theo thời gian
                var headerIdAccept = UnitOfWork.Repository<Repository.Implement.PS.VolumeAcceptRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == true && x.STATUS == "05").ToList();
                var detailAcceptVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeAcceptDetailRepo>().GetAll().ToList();
                var allDetailByHeaderIdAccept = (from x in detailAcceptVolume
                                                 join y in headerIdAccept on x.HEADER_ID equals y.ID
                                                 select new { timeId = y.TIME_PERIOD_ID, value = x.VALUE, price = x.PRICE }).ToList();
                var volumeAccept = (from x in allDetailByHeaderIdAccept
                                    group x by x.timeId into y
                                    select new
                                    {
                                        projectTimeId = y.Key,
                                        total = y.Sum(x => x.value * x.price),
                                    }).ToList();

                //merge data
                var data = (from x in (from x in allTimeInProject
                                       join y in planCost on x.ID equals y.projectTimeId
                                       select new { stringTime = (x.MONTH + "/" + x.YEAR).ToString(), time = x.ID, planCost = y.total }).ToList()
                            join y in (from x in allTimeInProject
                                       join y in volumeWork on x.ID equals y.projectTimeId into a
                                       from b in a.DefaultIfEmpty()
                                       select new { timeId = x.ID, volumeWork = b?.total ?? 0 }).ToList() on x.time equals y.timeId
                            join z in (from x in allTimeInProject
                                       join y in volumeAccept on x.ID equals y.projectTimeId into a
                                       from b in a.DefaultIfEmpty()
                                       select new { timeId = x.ID, volumeAccept = b?.total ?? 0 }).ToList() on x.time equals z.timeId

                            select new { x.stringTime, x.planCost, y.volumeWork, z.volumeAccept }).ToArray();

                return data;
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }

        public Array GetDataDashboardChiPhi(Guid projectId)
        {
            try
            {
                var allTimeInProject = UnitOfWork.Repository<Repository.Implement.PS.ProjectTimeRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).OrderBy(x => x.C_ORDER).ToList();

                //Lấy giá trị kế hoạch theo thời gian
                var planCostChiPhi = UnitOfWork.Repository<Repository.Implement.PS.PlanCostRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == false).ToList();
                var lstProjectStruct = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId);
                var allPlanCostByTimeId = (from x in planCostChiPhi
                                           join y in lstProjectStruct on x.PROJECT_STRUCT_ID equals y.ID
                                           select new { timeId = x.TIME_PERIOD_ID, value = x.VALUE, price = y.PRICE }).ToList();
                var planCost = (from x in allPlanCostByTimeId
                                group x by x.timeId into y
                                select new
                                {
                                    projectTimeId = y.Key,
                                    total = y.Sum(x => x.value * x.price),
                                }).ToList();

                //Lấy giá trị thực hiện theo thời gian
                var headerIdWork = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == false && x.STATUS == "05").ToList();
                var detailWorkVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkDetailRepo>().GetAll().ToList();
                var allDetailByHeaderIdWork = (from x in detailWorkVolume
                                               join y in headerIdWork on x.HEADER_ID equals y.ID
                                               select new { timeId = y.TIME_PERIOD_ID, value = x.VALUE, price = x.PRICE }).ToList();
                var volumeWork = (from x in allDetailByHeaderIdWork
                                  group x by x.timeId into y
                                  select new
                                  {
                                      projectTimeId = y.Key,
                                      total = y.Sum(x => x.value * x.price),
                                  }).ToList();

                //Lấy giá trị SPI theo kỳ dự án
                var planCostBOQ = UnitOfWork.Repository<Repository.Implement.PS.PlanCostRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == true).ToList();
                var lstProjectStructBOQ = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId);
                var allPlanCostBOQByTimeId = (from x in planCostBOQ
                                              join y in lstProjectStructBOQ on x.PROJECT_STRUCT_ID equals y.ID
                                              select new { timeId = x.TIME_PERIOD_ID, value = x.VALUE, price = y.PRICE }).ToList();
                var lstWP = (from x in allPlanCostBOQByTimeId
                             group x by x.timeId into y
                             select new
                             {
                                 projectTimeId = y.Key,
                                 total = y.Sum(x => x.value * x.price),
                             }).ToList();

                var headerIdWorkBOQ = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == true && x.STATUS == "05").ToList();
                var detailWorkVolumeBOQ = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkDetailRepo>().GetAll().ToList();
                var allDetailByHeaderIdWorkBOQ = (from x in detailWorkVolumeBOQ
                                                  join y in headerIdWorkBOQ on x.HEADER_ID equals y.ID
                                                  select new { timeId = y.TIME_PERIOD_ID, value = x.VALUE, price = x.PRICE }).ToList();
                var lstWD = (from x in allDetailByHeaderIdWorkBOQ
                             group x by x.timeId into y
                             select new
                             {
                                 projectTimeId = y.Key,
                                 total = y.Sum(x => x.value * x.price),
                             }).ToList();

                var lstSPI = (from x in lstWD
                              join y in lstWP on x.projectTimeId equals y.projectTimeId
                              select new
                              {
                                  projectTimeId = x.projectTimeId,
                                  spi = (x.total > 0 && y.total > 0) ? x.total / y.total : 0,
                              }).ToList();

                //merge data
                var data = (from x in (from x in allTimeInProject
                                       join y in planCost on x.ID equals y.projectTimeId
                                       select new { stringTime = (x.MONTH + "/" + x.YEAR).ToString(), time = x.ID, planCost = y.total }).ToList()
                            join y in (from x in allTimeInProject
                                       join y in volumeWork on x.ID equals y.projectTimeId into a
                                       from b in a.DefaultIfEmpty()
                                       select new { timeId = x.ID, volumeWork = b?.total ?? 0 }).ToList() on x.time equals y.timeId
                            join z in (from x in allTimeInProject
                                       join y in lstSPI on x.ID equals y.projectTimeId into a
                                       from b in a.DefaultIfEmpty()
                                       select new { timeId = x.ID, spi = b?.spi ?? 0 }).ToList() on x.time equals z.timeId

                            select new { x.stringTime, x.planCost, y.volumeWork, bcwp = z.spi * x.planCost }).ToArray();

                return data;
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }

        }

        public ArrayList GetDataCostLevel2(Guid projectId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var idParent = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.TYPE != "BOQ" && x.PARENT_ID != null && x.GEN_CODE == ObjDetail.CODE).ToList();

                var taskCostLevel2 = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PARENT_ID == idParent.FirstOrDefault().ID).ToList();
                taskCostLevel2.Insert(0, idParent.FirstOrDefault());

                var headerIdWork = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.IS_CUSTOMER == false && x.STATUS == "05" && x.TO_DATE <= toDate /* && fromDate <= x.TO_DATE && toDate >= x.TO_DATE*/).ToList();
                var detailWorkVolume = UnitOfWork.Repository<Repository.Implement.PS.VolumeWorkDetailRepo>().GetAll().ToList();
                var allDetailByHeaderIdWork = (from x in detailWorkVolume
                                               join y in headerIdWork on x.HEADER_ID equals y.ID
                                               select new { projectStructId = x.PROJECT_STRUCT_ID, total = x.TOTAL }).ToList();
                var volumeWork = (from x in allDetailByHeaderIdWork
                                  group x by x.projectStructId into y
                                  select new
                                  {
                                      projectStructId = y.Key,
                                      total = y.Sum(x => x.total)
                                  }).ToList();

                var allProjectStructVendor = UnitOfWork.Repository<Repository.Implement.PS.ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.TYPE != "BOQ").ToList();
                var mergeWork = new ArrayList();

                var valuePlanCost = (from x in UnitOfWork.Repository<Repository.Implement.PS.PlanCostRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.TimePeriod.FINISH_DATE <= toDate /*&& fromDate <= x.TimePeriod.FINISH_DATE && toDate >= x.TimePeriod.FINISH_DATE*/ && x.IS_CUSTOMER == false)
                                     group x by x.PROJECT_STRUCT_ID into y
                                     select new
                                     {
                                         projectStructId = y.Key,
                                         value = y.Sum(x => x.VALUE),
                                     }).ToList();

                foreach (var item in taskCostLevel2.OrderBy(x => x.C_ORDER))
                {
                    var lookup = allProjectStructVendor.ToLookup(x => x.PARENT_ID);
                    var res = lookup[item.ID].SelectRecursive(x => lookup[x.ID]).ToList();
                    if (res.FirstOrDefault() == null)
                    {
                        res.Add(item);
                    }
                    var sumPlanCostVendor = res.Sum(x => x.QUANTITY * x.PRICE);
                    var sumVolumeWork = (from x in volumeWork
                                         join y in res on x.projectStructId equals y.ID
                                         select new { total = decimal.Round(x.total, 0, MidpointRounding.AwayFromZero) }).Sum(x => x.total);
                    var CostData = (from x in res
                                    join y in valuePlanCost on x.ID equals y.projectStructId
                                    select new { total = x.PRICE * y.value }).Sum(x => x.total);
                    var data = new
                    {
                        Text = item.TEXT,
                        Code = item.GEN_CODE,
                        CostData = CostData,
                        SumPlanCostVendor = sumPlanCostVendor,
                        SumVolumeWork = sumVolumeWork,
                    };
                    mergeWork.Add(data);
                }

                return mergeWork;
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xẩy ra trong quá trình xem dashboard:" +
                    "- Dự án chưa bắt đầu lập kế hoạch?" +
                    "- Lỗi trong quá trình tính toán?" +
                    "- Nguyên nhân khác!";
                this.Exception = ex;
                return null;
            }


        }
        public List<ProjectResourceData> GenerateProjectResourceData(ProjectResourceModel model)
        {
            var data = new List<ProjectResourceData>();
            var lstProject = UnitOfWork.Repository<ProjectRepo>().Queryable();
            if (!string.IsNullOrEmpty(model.CompanyId))
            {
                lstProject = lstProject.Where(x => x.DON_VI == model.CompanyId);
            }
            if (model.ProjectId!= null && model.ProjectId!=Guid.Empty)
            {
                lstProject = lstProject.Where(x => x.ID == model.ProjectId);
            }

            var count = 0;
            foreach (var project in lstProject)
            {               
                var userFecon = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x =>x.PROJECT_ID == project.ID && model.FromDate<= x.FROM_DATE && x.TO_DATE<= model.ToDate).ToList();
                if (!string.IsNullOrEmpty(model.Username))
                {
                    userFecon = userFecon.Where(x => x.User.FULL_NAME.ToLower().Contains(model.Username.ToLower())).ToList();
                }
             

                var otherUser = UnitOfWork.Repository<ProjectResourceOtherRepo>().Queryable().Where(x => x.PROJECT_ID == project.ID && model.FromDate <= x.FROM_DATE && x.TO_DATE <= model.ToDate).ToList();
                if (!string.IsNullOrEmpty(model.ResourceOther))
                {
                    otherUser = otherUser.Where(x => x.FULL_NAME.ToLower().Contains(model.ResourceOther.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(model.Role))
                {
                    userFecon = userFecon.Where(x =>x.ProjectRole != null? x.ProjectRole.NAME.ToLower().Contains(model.Role.ToLower()): x.PROJECT_ROLE_ID==model.Role).ToList();
                    otherUser = otherUser.Where(x => x.VAI_TRO!= null? x.VAI_TRO.ToLower().Contains(model.Role.ToLower()): x.VAI_TRO==model.Role).ToList();
                }

                switch (model.TypeResource)
                {
                    case "FECON":
                        if(userFecon.Count() != 0)
                        {
                            data.Add(new ProjectResourceData
                            {
                                Username = project.CODE.ToUpper() + " - " + project.NAME.ToUpper(),
                                Stt = null,
                            });
                        }
                        break;
                    case "OTHER":
                        if (otherUser.Count() != 0) {

                            data.Add(new ProjectResourceData
                            {
                                Username = project.CODE.ToUpper() + " - " + project.NAME.ToUpper(),
                                Stt = null,
                            });
                        }
                        break;
                    default: 
                        if(otherUser.Count() + userFecon.Count() != 0)
                        {
                            data.Add(new ProjectResourceData
                            {
                                Username = project.CODE.ToUpper() + " - " + project.NAME.ToUpper(),
                                Stt = null,
                            });
                        }
                        break;
                } 
                if (string.IsNullOrEmpty(model.TypeResource) || model.TypeResource == "FECON")
                {
                    foreach (var user in userFecon)
                    {
                        var item = new ProjectResourceData
                        {
                            Stt = count++,
                            Username = user.User?.FULL_NAME,
                            ProjectRole = user.ProjectRole?.NAME,
                            PhoneNumber = user.User?.PHONE,
                            Email = user.User?.EMAIL,
                            TypeResource = "Nhân sự FECON",
                            OtherResource = null,
                            Department = user.User?.Organize?.NAME,
                            TitleResource = user.User?.Title?.NAME,
                            NumberCccd = null,
                            FromDate = user.FROM_DATE,
                            ToDate = user.TO_DATE,
                        };                       
                        data.Add(item);                   
                    }
                }
                if (string.IsNullOrEmpty(model.TypeResource) || model.TypeResource == "OTHER")
                {
                    foreach (var other in otherUser)
                    {
                        var item = new ProjectResourceData
                        {
                            Stt = count++,
                            Username = other.FULL_NAME,
                            ProjectRole = other.VAI_TRO,
                            PhoneNumber = other.PHONE,  
                            Email = other.EMAIL,
                            TypeResource = "Đối tác",
                            OtherResource = other.FULL_NAME,
                            Department = null,
                            TitleResource = null,
                            NumberCccd = other.CMT,
                            FromDate = other.FROM_DATE,
                            ToDate = other.TO_DATE,
                        };                       
                       data.Add(item);                      
                    }
                }
            }
            return data;
        }
        internal void UpdateConfigHideColumn(ConfigHideColumnModels model)
        {
            try
            {
                var item = (model.ProjectId == null || model.ProjectId == Guid.Empty) ? UnitOfWork.Repository<ConfigHideColumnRepo>().Queryable().FirstOrDefault(x => x.USER_NAME == ProfileUtilities.User.USER_NAME && x.TYPE_DISPLAY == model.Display) : UnitOfWork.Repository<ConfigHideColumnRepo>().Queryable().FirstOrDefault(x => x.USER_NAME == ProfileUtilities.User.USER_NAME && x.PROJECT_ID == model.ProjectId && x.TYPE_DISPLAY == model.Display);
                
                UnitOfWork.Clear();
                UnitOfWork.BeginTransaction();

                if(item != null) {
                    item.DETAILS= model.Details;

                    UnitOfWork.Repository<ConfigHideColumnRepo>().Delete(item);
                }
                else
                {
                    UnitOfWork.Repository<ConfigHideColumnRepo>().Create(new T_PS_CONFIG_HIDE_COLUMN
                    {
                        USER_NAME= ProfileUtilities.User.USER_NAME,
                        PROJECT_ID= model.ProjectId,
                        TYPE_DISPLAY= model.Display,
                        DETAILS= model.Details,
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
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var parent in source)
            {
                yield return parent;
                var children = selector(parent);
                foreach (var child in SelectRecursive(children, selector))
                    yield return child;
            }
        }
    }
}