using NHibernate.Linq;

using SMO.AppCode.GanttChart;
using SMO.AppCode.GranttChart;
using SMO.AppCode.Utilities;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;
using SMO.SAPINT.Class;
using SMO.SAPINT;
using SMO.Service.AD;
using SMO.Service.PS.Models;
using SMO.Service.PS.ValidateImport;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.ModelBinding;
using static SMO.SAPINT.Functions.FunctionPS;
using SharpSapRfc;
using SharpSapRfc.Plain;

namespace SMO.Service.PS
{
    public class ProjectStructService : GenericService<T_PS_PROJECT_STRUCT, ProjectStructRepo>
    {
        public override void Search()
        {
            NumerRecordPerPage = int.MaxValue;
            base.Search();
        }
        public ProjectStructureType Type { get; set; }
        public void ImportMsProject(HttpRequestBase request)
        {
            var project = UnitOfWork.Repository<ProjectRepo>().Get(ObjDetail.PROJECT_ID);
            if (project.STATUS == ProjectStatus.BAT_DAU.GetValue())
            {
                State = false;
                ErrorMessage = "Không thể Import dữ liệu khi dự án đã ở trạng thái Đang thực hiện.";
                return;
            }

            var now = DateTime.Now;
            var fileName = Guid.NewGuid().ToString("N");
            var pathSaveFile = Path.Combine(WebConfigurationManager.AppSettings["ImportProjectPath"], now.Year.ToString(), now.Month.ToString(), now.Day.ToString());
            if (!new DirectoryInfo(pathSaveFile).Exists)
            {
                Directory.CreateDirectory(pathSaveFile);
            }
            var pathFile = Path.Combine(pathSaveFile, $"{fileName}.xlsx");
            request.Files[0].SaveAs(pathFile);

            //var msProjectHelper = new MsProjectHelper(pathFile);
            var tableData = ExcelDataExchange.ReadData(pathFile);
            var projectStructure = CurrentRepository.Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID && x.TYPE == ProjectEnum.PROJECT.ToString())
                .FirstOrDefault();

            var units = UnitOfWork.Repository<Repository.Implement.MD.UnitRepo>().Queryable()
                .Select(x => x.CODE)
                .ToList();
            var validateResult = new ValidateImportProject().ValidateData(tableData, project.CODE,
                isStructureCost: Type == ProjectStructureType.COST,
                unitCodes: units);
            if (validateResult.Count() > 0)
            {
                ErrorMessage = new ValidateImportProject().GetValidateMessage(validateResult, isStructureCost: Type == ProjectStructureType.COST);
                State = false;
                return;
            }
            var currentUser = ProfileUtilities.User?.USER_NAME;
            try
            {
                var dictKeys = new Dictionary<string, ValueTuple<Guid, Guid>>();

                var activityRepo = UnitOfWork.Repository<ActivityRepo>();
                var wbsRepo = UnitOfWork.Repository<WbsRepo>();
                var boqRepo = UnitOfWork.Repository<BoqRepo>();
                UnitOfWork.BeginTransaction();
                DeleteDataCurrentProject();
                //ValidateData();
                for (int i = 2; i < tableData.Rows.Count; i++)
                {
                    var code = tableData.Rows[i][0].ToString();
                    var parentCode = tableData.Rows[i][1].ToString();
                    var name = tableData.Rows[i][2].ToString();
                    DateTime startDate;
                    DateTime finishDate;
                    var canParseStartDate = DateTime.TryParseExact(tableData.Rows[i][3].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
                    var canParseFinishDate = DateTime.TryParseExact(tableData.Rows[i][4].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out finishDate);

                    var type = tableData.Rows[i][5].ToString().GetEnumByName<ProjectEnum>();
                    var quantity = UtilsCore.StringToDecimal(tableData.Rows[i][6].ToString());
                    var price = UtilsCore.StringToDecimal(tableData.Rows[i][7].ToString());
                    var unit = tableData.Rows[i][8].ToString()?.ToUpper();

                    var dbId = Guid.NewGuid();
                    var elementId = Guid.NewGuid();
                    ValueTuple<Guid, Guid> parent;
                    dictKeys.Add(code, ValueTuple.Create(dbId, elementId));

                    if (unit == "%")
                    {
                        quantity /= 100;
                    }
                    var structure = new T_PS_PROJECT_STRUCT
                    {
                        ID = dbId,
                        TYPE = type.ToString(),
                        GEN_CODE = code,
                        START_DATE = startDate,
                        STATUS = ProjectStructStatus.KHOI_TAO.GetValue(),
                        FINISH_DATE = finishDate,
                        C_ORDER = i,
                        TEXT = name,
                        ACTIVE = true,
                        QUANTITY = Convert.ToDecimal(quantity.ToString("0.000")),
                        PRICE = price,
                        TOTAL = quantity * price,
                        UNIT_CODE = unit,
                        CREATE_BY = currentUser,
                        PROJECT_ID = ObjDetail.PROJECT_ID,
                        PARENT_ID = dictKeys.TryGetValue(parentCode, out parent) ? parent.Item1 : projectStructure.ID,
                        PLAN_VOLUME = quantity
                    };

                    UnitOfWork.Repository<GenCodeHistoryRepo>().Create(new T_PS_GEN_CODE_HISTORY
                    {
                        ID = Guid.NewGuid(),
                        TYPE = structure.TYPE,
                        GEN_CODE = structure.GEN_CODE,
                        PROJECT_ID = structure.PROJECT_ID
                    });

                    switch (type)
                    {
                        case ProjectEnum.WBS:
                            var wbs = new T_PS_WBS
                            {
                                ID = elementId,
                                CODE = code,
                                START_DATE = startDate,
                                FINISH_DATE = finishDate,
                                REFERENCE_FILE_ID = Guid.NewGuid(),
                                CREATE_BY = currentUser,
                                TEXT = name,
                                PROJECT_ID = ObjDetail.PROJECT_ID,
                                WBS_PARENT_ID = dictKeys.TryGetValue(parentCode, out parent) ? parent.Item2 : ObjDetail.PROJECT_ID,
                            };
                            structure.Wbs = wbs;
                            structure.WBS_ID = wbs.ID;

                            wbsRepo.Create(wbs);
                            break;
                        case ProjectEnum.ACTIVITY:
                            var activity = new T_PS_ACTIVITY
                            {
                                ID = elementId,
                                CODE = code,
                                START_DATE = startDate,
                                FINISH_DATE = finishDate,
                                REFERENCE_FILE_ID = Guid.NewGuid(),
                                CREATE_BY = currentUser,
                                TEXT = name,
                                PROJECT_ID = ObjDetail.PROJECT_ID,
                                WBS_PARENT_ID = dictKeys.TryGetValue(parentCode, out parent) ? parent.Item2 : ObjDetail.PROJECT_ID,
                            };
                            structure.ACTIVITY_ID = activity.ID;
                            structure.WBS_ID = activity.WBS_PARENT_ID;
                            structure.Activity = activity;

                            activityRepo.Create(activity);
                            break;
                        case ProjectEnum.BOQ:
                            var boq = new T_PS_BOQ
                            {
                                ID = elementId,
                                START_DATE = startDate,
                                FINISH_DATE = finishDate,
                                CODE = code,
                                REFERENCE_FILE_ID = Guid.NewGuid(),
                                CREATE_BY = currentUser,
                                TEXT = name,
                                PROJECT_ID = ObjDetail.PROJECT_ID,
                            };
                            structure.Boq = boq;
                            structure.BOQ_ID = boq.ID;

                            boqRepo.Create(boq);
                            break;
                        default:
                            break;
                    }

                    CurrentRepository.Create(structure);
                }
                UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, ObjDetail.UPDATE_BY, "Cây cấu trúc dự án");
                UnitOfWork.Repository<ProjectRepo>().Queryable().Where(x => x.ID == ObjDetail.PROJECT_ID)
                    .Update(x => new T_PS_PROJECT
                    {
                        FILE_NAME = $"{fileName}.xlsx"
                    });
                UnitOfWork.Commit();
                UpdateTotalStructure(ObjDetail.PROJECT_ID);
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        private void UpdateTotalStructure(Guid projectId)
        {
            try
            {
                var allStructures = CurrentRepository.Queryable()
                    .Where(x => x.PROJECT_ID == projectId)
                    .ToList();

                UnitOfWork.BeginTransaction();
                foreach (var structure in allStructures.Where(x => x.TOTAL == 0).OrderByDescending(x => x.C_ORDER))
                {
                    structure.TOTAL = allStructures.Where(x => x.PARENT_ID == structure.ID).Sum(x => x.TOTAL);
                    CurrentRepository.Queryable().Where(x => x.ID == structure.ID)
                        .Update(x => new T_PS_PROJECT_STRUCT
                        {
                            TOTAL = structure.TOTAL
                        });
                    if (structure.TYPE == ProjectEnum.PROJECT.ToString())
                    {
                        var isBoq = Type == ProjectStructureType.BOQ;

                        UnitOfWork.Repository<ProjectRepo>().Queryable()
                            .Where(x => x.ID == projectId)
                            .Update(x => new T_PS_PROJECT
                            {
                                TOTAL_BOQ = isBoq ? structure.TOTAL : null,
                                TOTAL_COST = isBoq ? null : structure.TOTAL,
                            });
                    }
                }
                UnitOfWork.Commit();

            }
            catch (Exception)
            {
                UnitOfWork.Rollback();
            }

        }

        internal void UpdateTasksTotal(bool isCustomer, IEnumerable<UpdateTasksTotalDto> data)
        {
            try
            {
                if (data.Count() == 0)
                {
                    return;
                }
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var projectId = data.Where(x => x.ProjectId != Guid.Empty).First().ProjectId;
                var projectStructureId = CurrentRepository.Queryable()
                    .Where(x => x.TYPE == ProjectEnum.PROJECT.ToString() && x.PROJECT_ID == projectId)
                    .Select(x => x.ID)
                    .FirstOrDefault();
                foreach (var item in data)
                {
                    CurrentRepository.Queryable().Where(x => x.ID == item.Id)
                        .Update(x => new T_PS_PROJECT_STRUCT
                        {
                            TOTAL = item.Total,
                            UPDATE_BY = currentUser,
                        });
                    if (item.Id == projectStructureId)
                    {
                        UnitOfWork.Repository<ProjectRepo>().Queryable()
                            .Where(x => x.ID == projectId)
                            .Update(x => new T_PS_PROJECT
                            {
                                TOTAL_BOQ = isCustomer ? item.Total : x.TOTAL_BOQ,
                                TOTAL_COST = isCustomer ? x.TOTAL_COST : item.Total,
                            });
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

        internal void UpdateTasksOrder(UpdateTasksOrderDto tasksOrderDto)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                for (int i = 0; i < tasksOrderDto.TaskIds.Count; i++)
                {
                    CurrentRepository.Queryable()
                        .Where(x => x.ID == tasksOrderDto.TaskIds[i])
                        .Update(x => new T_PS_PROJECT_STRUCT
                        {
                            UPDATE_BY = currentUser,
                            C_ORDER = i
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

        internal void InitContractCode(List<TaskDto> taskDtos, bool isCostStructure)
        {
            var contracts = GetAllContracts(isCostStructure);
            foreach (var contract in contracts)
            {
                foreach (var contractDetail in contract.Details)
                {
                    var structure = taskDtos.FirstOrDefault(x => x.Id == contractDetail.PROJECT_STRUCT_ID);
                    if (structure != null)
                    {
                        structure.ContractCode = contract.CONTRACT_NUMBER;
                        structure.VendorName = !contract.IS_SIGN_WITH_CUSTOMER ? contract.Vendor.NAME : string.Empty;
                    }
                }
            }
        }

        private IList<T_PS_CONTRACT> GetAllContracts(bool isCostStructure)
        {
            return UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID && x.IS_SIGN_WITH_CUSTOMER != isCostStructure)
                .ToList();
        }

        private void DeleteDataCurrentProject()
        {
            var tables = new string[]
            {
                nameof(T_PS_WBS),
                nameof(T_PS_ACTIVITY)
            };
            var isBoq = Type == ProjectStructureType.BOQ;
            if (isBoq)
            {
                tables = new string[]
                {
                    nameof(T_PS_BOQ)
                };
            }

            var contractIds = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID && x.IS_SIGN_WITH_CUSTOMER == isBoq)
                .Select(x => x.ID)
                .ToList();
            UnitOfWork.Repository<ContractDetailRepo>().Queryable()
                .Where(x => contractIds.Contains(x.CONTRACT_ID))
                .Delete();

            foreach (var table in tables)
            {
                StringBuilder hqlDump = new StringBuilder();
                hqlDump.Append(string.Format("DELETE FROM {0} a ", table));
                hqlDump.Append("WHERE a.PROJECT_ID = ?");
                UnitOfWork.GetSession().CreateQuery(hqlDump.ToString())
                    .SetParameter(0, ObjDetail.PROJECT_ID)
                    .ExecuteUpdate();
            }
            StringBuilder hql = new StringBuilder();
            hql.Append(string.Format("DELETE FROM {0} a ", nameof(T_PS_PROJECT_STRUCT)));
            hql.Append($"WHERE a.{nameof(T_PS_PROJECT_STRUCT.PROJECT_ID)} = ? ");
            if (Type == ProjectStructureType.BOQ)
            {
                hql.Append($" AND a.TYPE = 'BOQ'");
            }
            else
            {
                hql.Append($" AND (a.TYPE = 'WBS' OR a.TYPE = 'ACTIVITY')");
            }
            UnitOfWork.GetSession().CreateQuery(hql.ToString())
                .SetParameter(0, ObjDetail.PROJECT_ID)
                .ExecuteUpdate();

        }

        public override void Get(object id, dynamic param = null)
        {

            try
            {
                this.ObjDetail = this.CurrentRepository.Get(id, param);
                if (ObjDetail != null)
                {
                    this.ObjDetail.ContractDetail = UnitOfWork.Repository<ContractDetailRepo>()
                            .Queryable()
                            .Where(x => x.PROJECT_STRUCT_ID == ObjDetail.ID)
                            .FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
        public void Update(TaskDto taskDto)
        {
            try
            {
                var currentObj = CurrentRepository.Get(ObjDetail.ID);
                var projectStatus = UnitOfWork.Repository<ProjectRepo>().Get(currentObj.PROJECT_ID).STATUS;

                CurrentRepository.Detach(currentObj);

                UnitOfWork.BeginTransaction();
                this.ObjDetail.UPDATE_BY = taskDto.User;
                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                }
                if (DateTime.Compare(taskDto.End_date, currentObj.START_DATE) < 0)
                {
                    ErrorMessage = "Lỗi ngày kết thúc < ngày bắt đầu";
                    State = false;
                    return;
                }
                var hasDataPlanCost = UnitOfWork.Repository<PlanCostRepo>().Queryable().Where(x => x.PROJECT_STRUCT_ID == taskDto.Id && taskDto.End_date < x.TimePeriod.FINISH_DATE);

                if (hasDataPlanCost.FirstOrDefault() != null)
                {
                    if (Convert.ToInt32(hasDataPlanCost.Sum(x => x.VALUE)) > 0)
                    {
                        ErrorMessage = "Hạng mục đã phát sinh dữ liệu số kế hoạch và số thực tế trong khoảng từ ngày - " + taskDto.End_date.ToString("dd/MM/yyyy") + " - đến ngày - " + currentObj.FINISH_DATE.ToString("dd/MM/yyyy") + " - ! Vui lòng kiểm tra lại!";
                        State = false;
                        return;
                    }
                }
                if (taskDto.Price < 0)
                {
                    ErrorMessage = "Vui lòng nhập Đơn giá lớn hơn 0!";
                    State = false;
                    return;
                }
                if (currentObj.STATUS != taskDto.Status)
                {
                    switch ((ProjectEnum)Enum.Parse(typeof(ProjectEnum), currentObj.TYPE))
                    {
                        case ProjectEnum.WBS:
                            var wbs = UnitOfWork.Repository<WbsRepo>().Get(currentObj.WBS_ID);
                            wbs.TEXT = taskDto.Text;
                            wbs.START_DATE = taskDto.Start_date;
                            wbs.FINISH_DATE = taskDto.End_date;
                            wbs.BOQ_REFRENCE_ID = taskDto.ReferenceBoqId;
                            wbs.CODE = taskDto.Code;
                            wbs.STATUS = taskDto.Status;
                            wbs.UPDATE_BY = taskDto.User;
                            UnitOfWork.Repository<WbsRepo>().Update(wbs);
                            break;
                        case ProjectEnum.BOQ:
                            var boq = UnitOfWork.Repository<BoqRepo>().Get(currentObj.BOQ_ID);
                            boq.TEXT = taskDto.Text;
                            boq.START_DATE = taskDto.Start_date;
                            boq.FINISH_DATE = taskDto.End_date;
                            boq.CODE = taskDto.Code;
                            boq.STATUS = taskDto.Status;
                            boq.UPDATE_BY = taskDto.User;
                            //boq.PLAN_VOLUME = ObjDetail.QUANTITY.HasValue ? ObjDetail.QUANTITY.Value : 0;
                            UnitOfWork.Repository<BoqRepo>().Update(boq);
                            break;
                        case ProjectEnum.ACTIVITY:
                            var activity = UnitOfWork.Repository<ActivityRepo>().Get(currentObj.ACTIVITY_ID);
                            activity.TEXT = taskDto.Text;
                            activity.START_DATE = taskDto.Start_date;
                            activity.FINISH_DATE = taskDto.End_date;
                            activity.CODE = taskDto.Code;
                            activity.BOQ_REFRENCE_ID = taskDto.ReferenceBoqId;
                            activity.STATUS = taskDto.Status;
                            activity.UPDATE_BY = taskDto.User;
                            UnitOfWork.Repository<ActivityRepo>().Update(activity);
                            break;
                        case ProjectEnum.PROJECT:
                            var project = UnitOfWork.Repository<ProjectRepo>().Get(currentObj.PROJECT_ID);
                            project.NAME = taskDto.Text;
                            project.START_DATE = taskDto.Start_date;
                            project.FINISH_DATE = taskDto.End_date;
                            project.UPDATE_BY = taskDto.User;
                            UnitOfWork.Repository<ProjectRepo>().Update(project);
                            break;
                    }
                    ObjDetail.ACTIVE = currentObj.ACTIVE;

                    currentObj.TEXT = ObjDetail.TEXT;
                    currentObj.FINISH_DATE = ObjDetail.FINISH_DATE;
                    currentObj.GEN_CODE = ObjDetail.GEN_CODE;
                    currentObj.START_DATE = ObjDetail.START_DATE;
                    currentObj.PARENT_ID = ObjDetail.PARENT_ID;
                    currentObj.UNIT_CODE = ObjDetail.UNIT_CODE;
                    currentObj.PRICE = ObjDetail.PRICE;
                    currentObj.QUANTITY = ObjDetail.QUANTITY;
                    currentObj.STATUS = ObjDetail.STATUS;
                    currentObj.UPDATE_BY = taskDto.User;

                    if (projectStatus == ProjectStatus.KHOI_TAO.GetValue() || projectStatus == ProjectStatus.LAP_KE_HOACH.GetValue())
                    {
                        currentObj.PLAN_VOLUME = ObjDetail.QUANTITY;
                    }

                    this.ObjDetail = this.CurrentRepository.Update(currentObj);

                    if (currentObj.IS_CREATE_ON_SAP)
                    {
                        UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == currentObj.ID).Delete();
                        UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                        {
                            ID = Guid.NewGuid(),
                            PROJECT_STRUCT_ID = currentObj.ID,
                            PROJECT_ID = currentObj.PROJECT_ID,
                            ACTION = "U"
                        });
                    }

                    UnitOfWork.Commit();
                }
                else
                {
                    UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, this.ObjDetail.UPDATE_BY, "Cây cấu trúc dự án");

                    switch ((ProjectEnum)Enum.Parse(typeof(ProjectEnum), currentObj.TYPE))
                    {
                        case ProjectEnum.WBS:
                            var wbs = UnitOfWork.Repository<WbsRepo>().Get(currentObj.WBS_ID);
                            wbs.TEXT = taskDto.Text;
                            wbs.START_DATE = taskDto.Start_date;
                            wbs.FINISH_DATE = taskDto.End_date;
                            wbs.BOQ_REFRENCE_ID = taskDto.ReferenceBoqId;
                            wbs.CODE = taskDto.Code;
                            wbs.STATUS = taskDto.Status;
                            wbs.UPDATE_BY = taskDto.User;
                            UnitOfWork.Repository<WbsRepo>().Update(wbs);
                            break;
                        case ProjectEnum.BOQ:
                            var boq = UnitOfWork.Repository<BoqRepo>().Get(currentObj.BOQ_ID);
                            boq.TEXT = taskDto.Text;
                            boq.START_DATE = taskDto.Start_date;
                            boq.FINISH_DATE = taskDto.End_date;
                            boq.CODE = taskDto.Code;
                            boq.STATUS = taskDto.Status;
                            boq.UPDATE_BY = taskDto.User;
                            //boq.PLAN_VOLUME = ObjDetail.QUANTITY.HasValue ? ObjDetail.QUANTITY.Value : 0;
                            UnitOfWork.Repository<BoqRepo>().Update(boq);
                            break;
                        case ProjectEnum.ACTIVITY:
                            var activity = UnitOfWork.Repository<ActivityRepo>().Get(currentObj.ACTIVITY_ID);
                            activity.TEXT = taskDto.Text;
                            activity.START_DATE = taskDto.Start_date;
                            activity.FINISH_DATE = taskDto.End_date;
                            activity.CODE = taskDto.Code;
                            activity.BOQ_REFRENCE_ID = taskDto.ReferenceBoqId;
                            activity.STATUS = taskDto.Status;
                            activity.UPDATE_BY = taskDto.User;
                            UnitOfWork.Repository<ActivityRepo>().Update(activity);
                            break;
                        case ProjectEnum.PROJECT:
                            var project = UnitOfWork.Repository<ProjectRepo>().Get(currentObj.PROJECT_ID);
                            project.NAME = taskDto.Text;
                            project.START_DATE = taskDto.Start_date;
                            project.FINISH_DATE = taskDto.End_date;
                            project.UPDATE_BY = taskDto.User;
                            UnitOfWork.Repository<ProjectRepo>().Update(project);
                            break;
                    }
                    ObjDetail.ACTIVE = currentObj.ACTIVE;

                    currentObj.TEXT = ObjDetail.TEXT;
                    currentObj.FINISH_DATE = ObjDetail.FINISH_DATE;
                    currentObj.GEN_CODE = ObjDetail.GEN_CODE;
                    currentObj.START_DATE = ObjDetail.START_DATE;
                    currentObj.PARENT_ID = ObjDetail.PARENT_ID;
                    currentObj.UNIT_CODE = ObjDetail.UNIT_CODE;
                    currentObj.PRICE = ObjDetail.PRICE;
                    currentObj.QUANTITY = ObjDetail.QUANTITY;
                    currentObj.STATUS = ObjDetail.STATUS;
                    currentObj.UPDATE_BY = taskDto.User;

                    if (projectStatus == ProjectStatus.KHOI_TAO.GetValue() || projectStatus == ProjectStatus.LAP_KE_HOACH.GetValue())
                    {
                        currentObj.PLAN_VOLUME = ObjDetail.QUANTITY;
                    }

                    this.ObjDetail = this.CurrentRepository.Update(currentObj);

                    if (currentObj.IS_CREATE_ON_SAP)
                    {
                        UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == currentObj.ID).Delete();
                        UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                        {
                            ID = Guid.NewGuid(),
                            PROJECT_STRUCT_ID = currentObj.ID,
                            PROJECT_ID = currentObj.PROJECT_ID,
                            ACTION = "U"
                        });
                    }

                    UnitOfWork.Commit();
                }

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal List<T_PS_PROJECT_STRUCT> FindChild(Guid parent)
        {
            List<T_PS_PROJECT_STRUCT> lstItem = new List<T_PS_PROJECT_STRUCT>();
            var allChild = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PARENT_ID == parent).ToList();
            if (allChild.Count > 0)
            {
                foreach (var item in allChild)
                {
                    lstItem.Add(item);
                    var child = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PARENT_ID == item.ID).ToList();
                    if (child.Count > 0)
                    {
                        FindChild(item.ID);
                    }
                }
            }

            return lstItem;
        }

        internal void Delete(Guid id, string currentUsername)
        {
            try
            {
                var currentObj = CurrentRepository.Get(ObjDetail.ID);

                var taskDelete = UnitOfWork.Repository<ProjectStructRepo>().Queryable().FirstOrDefault(x => x.ID == id);
                if (taskDelete.TYPE == "BOQ")
                {
                    var checkActivity = UnitOfWork.Repository<ActivityRepo>().Queryable().Any(x => x.BOQ_REFRENCE_ID == taskDelete.ID);
                    var checkWbs = UnitOfWork.Repository<WbsRepo>().Queryable().Any(x => x.BOQ_REFRENCE_ID == taskDelete.ID);
                    if (checkActivity || checkWbs)
                    {
                        ErrorMessage = "Không thể xoá BOQ đã liên kết với ACTIVITY | WBS!";
                        State = false;
                        return;
                    }
                }
                var allChild = FindChild(id);
                allChild.Add(taskDelete);
                var check = 0;

                foreach (var item in allChild)
                {
                    var hasWorkVolumeValue = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable().Any(x => x.PROJECT_STRUCT_ID == item.ID);
                    var hasAcceptVolumeValue = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable().Any(x => x.PROJECT_STRUCT_ID == item.ID);

                    if (hasWorkVolumeValue)
                    {
                        if (UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable().Where(x => x.PROJECT_STRUCT_ID == item.ID).Sum(x => x.VALUE) > 0)
                        {
                            check++;
                        }
                    }
                    if (hasAcceptVolumeValue)
                    {
                        if (UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable().Where(x => x.PROJECT_STRUCT_ID == item.ID).Sum(x => x.VALUE) > 0)
                        {
                            check++;
                        }
                    }
                }


                var checkSAP = false;
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                            systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new Update_Project_Function();
                    var structIdOfProject = taskDelete.PROJECT_ID;

                    var project = new ZST_BAPIPROJ()
                    {
                        PROJECT_CODE = taskDelete.Project.CODE,
                        COMPANY_CODE = taskDelete.Project.DonVi.COMPANY_CODE,
                        COST_CENTER_CODE = taskDelete.Project.DonVi.COST_CENTER_CODE,
                        PROJECT_NAME = taskDelete.Project.NAME,
                        FINISH_DATE = taskDelete.Project.FINISH_DATE,
                        START_DATE = taskDelete.Project.START_DATE,
                    };

                    var lstWbs = new List<ZST_BAPIWBS>();
                    foreach (var item in allChild.Where(x => x.TYPE == "WBS" && x.IS_CREATE_ON_SAP == true).ToList())
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
                    foreach (var item in allChild.Where(x => x.TYPE == "ACTIVITY" && x.IS_CREATE_ON_SAP == true).ToList())
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
                            PUR_GROUP = item.Project.PUR_GROUP
                        });
                    }

                    functionSAP.Parameters = new
                    {
                        PROJECT = project,
                        ADD = "",
                        CHANGE = "",
                        DELETE = "X",
                        WBS = lstWbs,
                        ACTIVITY = lstActivity
                    };

                    conn.ExecuteFunction(functionSAP);
                    var output = functionSAP.Result.GetTable<BAPIRET2>("EX_RETURN");
                    this.State = true;
                    if (output.Where(x => x.TYPE == "E").Count() > 0)
                    {
                        checkSAP = true;
                    }
                }

                if (check > 0 || checkSAP)
                {
                    ErrorMessage = "Một số hạng mục đã phát sinh dữ liệu thực tế trên PMS |SAP nên không thể xoá!";
                    State = false;
                    return;
                }
                else
                {
                    CurrentRepository.Detach(currentObj);
                    UnitOfWork.BeginTransaction();

                    foreach (var item in allChild)
                    {
                        if (item.IS_CREATE_ON_SAP)
                        {
                            UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == item.ID).Delete();
                        }
                        UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => x.ID == item.ID).Delete();
                        UnitOfWork.GetSession().Query<T_PS_CONTRACT_DETAIL>().Where(x => x.PROJECT_STRUCT_ID == item.ID).Delete();
                        // delete plan cost
                        UnitOfWork.GetSession().Query<T_PS_PLAN_COST>().Where(x => x.PROJECT_STRUCT_ID == item.ID).Delete();
                        // delete plan progress
                        UnitOfWork.GetSession().Query<T_PS_PLAN_PROGRESS>().Where(x => x.PROJECT_STRUCT_ID == item.ID).Delete();
                        // delete plan quantity
                        UnitOfWork.GetSession().Query<T_PS_PLAN_QUANTITY>().Where(x => x.PROJECT_STRUCT_ID == item.ID).Delete();
                        //delete volume work
                        UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK_DETAIL>().Where(x => x.PROJECT_STRUCT_ID == item.ID).Delete();
                        //delete volume accept
                        UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT_DETAIL>().Where(x => x.PROJECT_STRUCT_ID == item.ID).Delete();
                    }
                    UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, currentUsername, "Cây cấu trúc dự án");
                    UnitOfWork.Commit();
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal decimal CalculateTotalPlanVolume()
        {
            var plans = UnitOfWork.Repository<PlanQuantityRepo>().Queryable()
                .Where(x => x.PROJECT_STRUCT_ID == ObjDetail.ID)
                .ToList();
            return plans.Sum(x => x.VALUE);
        }
        internal decimal CalculateTotalWorkVolume()
        {
            //var works = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
            //    .Where(x => x.PROJECT_STRUCT_ID == ObjDetail.ID)
            //    .ToList();
            //return works.Sum(x => x.VALUE);
            return 0;
        }

        public override void Create()
        {
            try
            {
                this.ObjDetail.STATUS = ProjectStructStatus.KHOI_TAO.GetValue();
                UnitOfWork.BeginTransaction();
                switch ((ProjectEnum)Enum.Parse(typeof(ProjectEnum), ObjDetail.TYPE))
                {
                    case ProjectEnum.WBS:
                        var wbs = (T_PS_WBS)ObjDetail;
                        var project = UnitOfWork.Repository<ProjectRepo>().Get(ObjDetail.PROJECT_ID);
                        var projectStruct = CurrentRepository.Queryable()
                            .Where(x => x.TYPE == ProjectEnum.PROJECT.ToString() && x.PROJECT_ID == ObjDetail.PROJECT_ID)
                            .First();
                        string parentCode = project.CODE;
                        if (ObjDetail.PARENT_ID.HasValue && projectStruct.ID != ObjDetail.PARENT_ID)
                        {
                            parentCode = CurrentRepository.Queryable()
                                .Where(x => x.ID == ObjDetail.PARENT_ID.Value).Select(x => x.GEN_CODE)
                                .FirstOrDefault();
                        }
                        var regex = new Regex(string.Format("^{0}(\\.\\d{{2}})+$", project.CODE));

                        var wbses = CurrentRepository.Queryable()
                            .Where(x => x.TYPE == ProjectEnum.WBS.ToString()
                            && x.PROJECT_ID == ObjDetail.PROJECT_ID
                            && (x.GEN_CODE ?? string.Empty) != string.Empty)
                            .ToList();
                        var currentWbses = wbses.Where(x => regex.IsMatch(x.GEN_CODE)
                            && x.GEN_CODE.Substring(0, x.GEN_CODE.Length - 3) == parentCode)
                            .Select(x => int.Parse(x.GEN_CODE.Substring(x.GEN_CODE.Length - 2)))
                            .ToList();
                        int currentWbsOrder = currentWbses.Count > 0 ? currentWbses.Max() : 0;

                        wbs.REFERENCE_FILE_ID = Guid.NewGuid();
                        wbs.CREATE_BY = ProfileUtilities.User?.USER_NAME;
                        wbs.ACTIVE = true;
                        wbs.CODE = $"{parentCode}.{currentWbsOrder + 1:00}";
                        var createdWbs = UnitOfWork.Repository<WbsRepo>().Create(wbs);
                        ObjDetail.GEN_CODE = wbs.CODE;
                        ObjDetail.WBS_ID = createdWbs.ID;
                        break;
                    case ProjectEnum.BOQ:
                        var boq = (T_PS_BOQ)ObjDetail;
                        boq.REFERENCE_FILE_ID = Guid.NewGuid();
                        boq.CREATE_BY = ProfileUtilities.User?.USER_NAME;
                        boq.ACTIVE = true;
                        var createdBoq = UnitOfWork.Repository<BoqRepo>().Create(boq);
                        ObjDetail.BOQ_ID = createdBoq.ID;
                        break;
                    case ProjectEnum.ACTIVITY:
                        var currentActivities = UnitOfWork.Repository<GenCodeHistoryRepo>().Queryable()
                            .Where(x => x.TYPE == ProjectEnum.ACTIVITY.ToString() && x.PROJECT_ID == ObjDetail.PROJECT_ID && (x.GEN_CODE ?? string.Empty) != string.Empty)
                            .Select(x => int.Parse(x.GEN_CODE.Substring(x.GEN_CODE.Length - 4)))
                            .ToList();
                        var currentActivityOrder = currentActivities.Count > 0 ? currentActivities.Max() : 0;

                        var activity = (T_PS_ACTIVITY)ObjDetail;
                        activity.CREATE_BY = ProfileUtilities.User?.USER_NAME;
                        activity.REFERENCE_FILE_ID = Guid.NewGuid();
                        activity.ACTIVE = true;
                        activity.CODE = $"{currentActivityOrder + 1:0000}";
                        var createdActivity = UnitOfWork.Repository<ActivityRepo>().Create(activity);
                        ObjDetail.GEN_CODE = activity.CODE;
                        ObjDetail.ACTIVITY_ID = createdActivity.ID;
                        UnitOfWork.Repository<GenCodeHistoryRepo>().Create(new T_PS_GEN_CODE_HISTORY
                        {
                            ID = Guid.NewGuid(),
                            PROJECT_ID = ObjDetail.PROJECT_ID,
                            TYPE = ProjectEnum.ACTIVITY.ToString(),
                            GEN_CODE = activity.CODE
                        });
                        break;
                }
                ObjDetail.ACTIVE = true;
                CurrentRepository.Create(ObjDetail);

                UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                {
                    ID = Guid.NewGuid(),
                    PROJECT_STRUCT_ID = ObjDetail.ID,
                    PROJECT_ID = ObjDetail.PROJECT_ID,
                    ACTION = "A"
                });

                UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, ObjDetail.CREATE_BY, "Cây cấu trúc dự án");

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void SaveActivity()
        {
            try
            {
                var currentObj = CurrentRepository.Get(ObjDetail.ID);
                UnitOfWork.Repository<ActivityRepo>().Detach(currentObj.Activity);
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                ObjDetail.Activity.UPDATE_BY = currentUser;
                UnitOfWork.Repository<ActivityRepo>().Update(ObjDetail.Activity);

                currentObj.STATUS = ObjDetail.STATUS;
                currentObj.TEXT = ObjDetail.Activity.TEXT;
                currentObj.START_DATE = ObjDetail.Activity.START_DATE;
                currentObj.FINISH_DATE = ObjDetail.Activity.FINISH_DATE;
                currentObj.UPDATE_BY = currentUser;
                currentObj.QUANTITY = ObjDetail.QUANTITY;
                currentObj.PRICE = ObjDetail.PRICE;
                currentObj.UNIT_CODE = ObjDetail.UNIT_CODE;

                CurrentRepository.Update(currentObj);

                if (ObjDetail.IS_CREATE_ON_SAP)
                {
                    UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == ObjDetail.ID).Delete();
                    UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_STRUCT_ID = ObjDetail.ID,
                        PROJECT_ID = ObjDetail.PROJECT_ID,
                        ACTION = "U"
                    });
                }
                //UnitOfWork.Repository<ProjectRepo>().ResetStatus(currentObj.PROJECT_ID, currentUser, "Cây cấu trúc dự án");
                //UpdateContractDetailInformation();
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void SaveActivityFromContractVendor(List<AddNewTaskModel> lstTaskNew)
        {
            try
            {
                ErrorMessage = "";
                foreach (var item in lstTaskNew)
                {
                    this.UnitOfWork.Clear();
                    UnitOfWork.BeginTransaction();

                    if (DateTime.Parse(item.StartDate) > DateTime.Parse(item.EndDate))
                    {
                        ErrorMessage += $"Ngày kết thúc phải lớn hơn ngày bắt đầu tại hạng mục {item.Name}, ";
                        State = false;
                        continue;
                    }

                    var wbsParent = CurrentRepository.Queryable().Where(x => x.PROJECT_ID == item.ProjectId && x.TYPE == ProjectEnum.WBS.ToString() && x.GEN_CODE == item.Parent).FirstOrDefault();

                    var hasUnit = UnitOfWork.Repository<UnitRepo>().Queryable().Where(x => String.Equals(x.CODE, item.Unit)).FirstOrDefault();
                    if (hasUnit == null)
                    {
                        ErrorMessage += $"Đơn vị tính {item.Unit} không tồn tại tại hạng mục {item.Name}, ";
                        State = false;
                        continue;
                    }

                    var boqParent = new T_PS_PROJECT_STRUCT();
                    if (!String.IsNullOrEmpty(item.ParentBoq))
                    {
                        boqParent = CurrentRepository.Queryable().Where(x => x.PROJECT_ID == item.ProjectId && x.TYPE == ProjectEnum.BOQ.ToString() && x.GEN_CODE == item.ParentBoq).FirstOrDefault();
                        if (boqParent == null)
                        {
                            ErrorMessage = $"Mã BOQ liên kết {item.ParentBoq} không tồn tại tại hạng mục {item.Name}";
                            State = false;
                            continue;
                        }
                    }

                    if (wbsParent == null)
                    {
                        ErrorMessage = $"Mã hạng mục cha {item.Parent} không tồn tại tại hạng mục {item.Name}";
                        State = false;
                        continue;
                    }

                    var currentActivities = UnitOfWork.Repository<GenCodeHistoryRepo>().Queryable()
                                .Where(x => x.TYPE == ProjectEnum.ACTIVITY.ToString() && x.PROJECT_ID == item.ProjectId && (x.GEN_CODE ?? string.Empty) != string.Empty)
                                .Select(x => int.Parse(x.GEN_CODE.Substring(x.GEN_CODE.Length - 4)))
                                .ToList();
                    var currentActivityOrder = currentActivities.Count > 0 ? currentActivities.Max() : 0;

                    var lstParent = CurrentRepository.Queryable()
                                .Where(x => x.PROJECT_ID == item.ProjectId && x.PARENT_ID == wbsParent.ID)
                                .ToList().OrderBy(x => x.C_ORDER);

                    var activity = new T_PS_ACTIVITY()
                    {
                        ID = Guid.NewGuid(),
                        CREATE_BY = ProfileUtilities.User?.USER_NAME,
                        REFERENCE_FILE_ID = Guid.NewGuid(),
                        ACTIVE = true,
                        TEXT = item.Name,
                        CODE = $"{currentActivityOrder + 1:0000}",
                        PROJECT_ID = item.ProjectId,
                        START_DATE = DateTime.Parse(item.StartDate),
                        FINISH_DATE = DateTime.Parse(item.EndDate),
                        BOQ_REFRENCE_ID = item.ParentBoq != null ? boqParent.ID : (Guid?)null,
                        PEOPLE_RESPONSIBILITY = item.PeopleResponsibility
                    };

                    UnitOfWork.Repository<ActivityRepo>().Create(activity);

                    var task = new T_PS_PROJECT_STRUCT()
                    {
                        ID = Guid.NewGuid(),
                        PARENT_ID = wbsParent.ID,
                        GEN_CODE = activity.CODE,
                        ACTIVITY_ID = activity.ID,
                        WBS_ID = wbsParent.ID,
                        ACTIVE = true,
                        TYPE = ProjectEnum.ACTIVITY.ToString(),
                        C_ORDER = lstParent.LastOrDefault() == null ? wbsParent.C_ORDER + 0.5 : lstParent.LastOrDefault().C_ORDER + 0.5,
                        PROJECT_ID = item.ProjectId,
                        TEXT = item.Name,
                        START_DATE = DateTime.Parse(item.StartDate),
                        FINISH_DATE = DateTime.Parse(item.EndDate),
                        UNIT_CODE = item.Unit.ToUpper(),
                        QUANTITY = item.Unit.ToUpper() == "%" ? item.Quantity / 100 : item.Quantity,
                        PRICE = 0,
                        STATUS = "02",
                        IS_CREATE_ON_SAP = false,
                        CREATE_BY = ProfileUtilities.User.USER_NAME
                    };

                    UnitOfWork.Repository<GenCodeHistoryRepo>().Create(new T_PS_GEN_CODE_HISTORY
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = task.PROJECT_ID,
                        TYPE = task.TYPE,
                        GEN_CODE = activity.CODE
                    });

                    CurrentRepository.Create(task);

                    UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_STRUCT_ID = task.ID,
                        PROJECT_ID = item.ProjectId,
                        ACTION = "A",
                        CREATE_BY = ProfileUtilities.User.USER_NAME,
                    });

                    UnitOfWork.GetSession().Save(new T_PS_CONTRACT_DETAIL()
                    {
                        ID = Guid.NewGuid(),
                        CONTRACT_ID = item.ContractId,
                        PROJECT_STRUCT_ID = task.ID,
                        UNIT_CODE = task.UNIT_CODE,
                        VOLUME = item.Unit.ToUpper() == "%" ? item.Quantity / 100 : item.Quantity,
                        UNIT_PRICE = item.Price,
                        ACTIVE = true,
                        CREATE_BY = ProfileUtilities.User.USER_NAME,
                    });

                    UnitOfWork.Commit();

                    var allProjectStruct = CurrentRepository.Queryable().Where(x => x.PROJECT_ID == lstTaskNew.FirstOrDefault().ProjectId && x.TYPE != "BOQ").OrderBy(x => x.C_ORDER).Select(y => y.ID).ToList();
                    var lstTask = new UpdateTasksOrderDto();
                    lstTask.ProjectId = lstTaskNew.FirstOrDefault().ProjectId;
                    lstTask.TaskIds = allProjectStruct;

                    UpdateTasksOrder(lstTask);
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        private void UpdateContractDetailInformation()
        {
            var contractDetailRepo = UnitOfWork.Repository<ContractDetailRepo>();
            var contractDetail = contractDetailRepo
                .Queryable()
                .Where(x => x.PROJECT_STRUCT_ID == ObjDetail.ID)
                .FirstOrDefault();
            if (ObjDetail.ContractDetail?.CONTRACT_ID == Guid.Empty)
            {
                ObjDetail.ContractDetail = null;
            }
            if (contractDetail?.CONTRACT_ID != ObjDetail.ContractDetail?.CONTRACT_ID)
            {
                // update contract detail
                if (contractDetail == null)
                {
                    contractDetailRepo.Create(new T_PS_CONTRACT_DETAIL
                    {
                        CONTRACT_ID = ObjDetail.ContractDetail.CONTRACT_ID,
                        PROJECT_STRUCT_ID = ObjDetail.ID,
                        ID = Guid.NewGuid(),
                        CREATE_BY = ProfileUtilities.User?.USER_NAME
                    });
                }
                else if (ObjDetail.ContractDetail?.CONTRACT_ID == null)
                {
                    contractDetailRepo.Delete(contractDetail);
                }
                else
                {
                    contractDetail.CONTRACT_ID = ObjDetail.ContractDetail.CONTRACT_ID;
                    contractDetail.UNIT_CODE = null;
                    contractDetail.VOLUME = 0;
                    contractDetail.UNIT_PRICE = 0;
                    contractDetailRepo.Update(contractDetail);
                }
            }
        }

        internal void SaveProject()
        {
            try
            {
                var currentObj = CurrentRepository.Get(ObjDetail.ID);
                UnitOfWork.Repository<ProjectRepo>().Detach(currentObj.Project);
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                ObjDetail.Project.UPDATE_BY = currentUser;
                UnitOfWork.Repository<ProjectRepo>().Update(ObjDetail.Project);

                currentObj.TEXT = ObjDetail.Project.NAME;
                currentObj.START_DATE = ObjDetail.Project.START_DATE;
                currentObj.FINISH_DATE = ObjDetail.Project.FINISH_DATE;
                currentObj.UPDATE_BY = currentUser;
                CurrentRepository.Update(currentObj);

                UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == ObjDetail.ID).Delete();
                UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                {
                    ID = Guid.NewGuid(),
                    PROJECT_STRUCT_ID = ObjDetail.ID,
                    PROJECT_ID = ObjDetail.PROJECT_ID,
                    ACTION = "U"
                });
                if (ObjDetail.TYPE != "ACTIVITY")
                {
                    UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, currentUser, "Cây cấu trúc dự án");
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

        internal void SaveWbs()
        {
            try
            {
                var currentObj = CurrentRepository.Get(ObjDetail.ID);
                EnsureValidWbsCode(ObjDetail.Wbs.CODE);
                if (!State)
                {
                    return;
                }
                UnitOfWork.Repository<WbsRepo>().Detach(currentObj.Wbs);
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                ObjDetail.Wbs.UPDATE_BY = currentUser;
                UnitOfWork.Repository<WbsRepo>().Update(ObjDetail.Wbs);

                currentObj.STATUS = ObjDetail.STATUS;
                currentObj.TEXT = ObjDetail.Wbs.TEXT;
                currentObj.START_DATE = ObjDetail.Wbs.START_DATE;
                currentObj.FINISH_DATE = ObjDetail.Wbs.FINISH_DATE;
                currentObj.GEN_CODE = ObjDetail.Wbs.CODE;
                currentObj.UPDATE_BY = currentUser;
                currentObj.QUANTITY = ObjDetail.QUANTITY;
                currentObj.PRICE = ObjDetail.PRICE;
                currentObj.UNIT_CODE = ObjDetail.UNIT_CODE;

                CurrentRepository.Update(currentObj);

                if (ObjDetail.IS_CREATE_ON_SAP)
                {
                    UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == ObjDetail.ID).Delete();
                    UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_STRUCT_ID = ObjDetail.ID,
                        PROJECT_ID = ObjDetail.PROJECT_ID,
                        ACTION = "U"
                    });
                }
                UnitOfWork.Repository<ProjectRepo>().ResetStatus(currentObj.PROJECT_ID, currentUser, "Cây cấu trúc dự án");
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        private void EnsureValidWbsCode(string code)
        {

            var currentNumberCode = CurrentRepository.Queryable()
                .Where(x => x.GEN_CODE == code && x.ID != ObjDetail.ID && x.PROJECT_ID == ObjDetail.Wbs.PROJECT_ID)
                .Count();

            var projectCode = UnitOfWork.Repository<ProjectRepo>().Queryable()
                .Where(x => x.ID == ObjDetail.Wbs.PROJECT_ID)
                .Select(x => x.CODE)
                .FirstOrDefault();
            if (currentNumberCode > 1 || (code != projectCode && currentNumberCode == 1))
            {
                ErrorMessage = $"Mã code {code} đã tồn tại";
                State = false;
            }
        }

        internal void SaveBoq()
        {
            try
            {
                var currentObj = CurrentRepository.Get(ObjDetail.ID);
                UnitOfWork.Repository<BoqRepo>().Detach(currentObj.Boq);
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                ObjDetail.Boq.UPDATE_BY = currentUser;
                UnitOfWork.Repository<BoqRepo>().Update(ObjDetail.Boq);

                currentObj.GEN_CODE = ObjDetail.Boq.CODE;
                currentObj.STATUS = ObjDetail.STATUS;
                currentObj.TEXT = ObjDetail.Boq.TEXT;
                currentObj.START_DATE = ObjDetail.Boq.START_DATE;
                currentObj.FINISH_DATE = ObjDetail.Boq.FINISH_DATE;
                currentObj.QUANTITY = ObjDetail.QUANTITY;
                currentObj.PRICE = ObjDetail.PRICE;
                currentObj.UPDATE_BY = currentUser;
                currentObj.UNIT_CODE = ObjDetail.UNIT_CODE;
                CurrentRepository.Update(currentObj);
                //UpdateContractDetailInformation();
                UnitOfWork.Repository<ProjectRepo>().ResetStatus(currentObj.PROJECT_ID, currentUser, "Cây cấu trúc dự án");
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void UpdateStatuses(Guid projectId, string status, IList<Guid> structuresId)
        {
            try
            {
                var currentObjs = CurrentRepository.Queryable()
                    .Where(x => structuresId.Contains(x.ID))
                    .ToList();
                foreach (var obj in currentObjs)
                {
                    CurrentRepository.Detach(obj);
                }
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;

                CurrentRepository.Queryable()
                    .Where(x => structuresId.Contains(x.ID))
                    .Update(x => new T_PS_PROJECT_STRUCT
                    {
                        STATUS = status,
                        UPDATE_BY = currentUser
                    });

                var boqIds = currentObjs.Where(x => x.TYPE == ProjectEnum.BOQ.ToString()).Select(x => x.BOQ_ID).ToList();
                UnitOfWork.Repository<BoqRepo>().Queryable().Where(x => boqIds.Contains(x.ID)).Update(x => new T_PS_BOQ
                {
                    STATUS = status,
                    UPDATE_BY = currentUser
                });

                var activityIds = currentObjs.Where(x => x.TYPE == ProjectEnum.ACTIVITY.ToString()).Select(x => x.ACTIVITY_ID).ToList();
                UnitOfWork.Repository<ActivityRepo>().Queryable().Where(x => boqIds.Contains(x.ID)).Update(x => new T_PS_ACTIVITY
                {
                    STATUS = status,
                    UPDATE_BY = currentUser
                });

                var wbsIds = currentObjs.Where(x => x.TYPE == ProjectEnum.WBS.ToString()).Select(x => x.WBS_ID).ToList();
                UnitOfWork.Repository<WbsRepo>().Queryable().Where(x => boqIds.Contains(x.ID)).Update(x => new T_PS_WBS
                {
                    STATUS = status,
                    UPDATE_BY = currentUser
                });

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
