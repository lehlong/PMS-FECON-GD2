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
    public class ProjectStructDraftService : GenericService<T_PS_PROJECT_STRUCT_DRAFT, ProjectStructDraftRepo>
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
                    var structure = new T_PS_PROJECT_STRUCT_DRAFT
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

        public override void Create()
        {
            try
            {
                this.ObjDetail.STATUS = ProjectStructStatus.KHOI_TAO.GetValue();
                UnitOfWork.BeginTransaction();

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

                ObjDetail.GEN_CODE = $"{parentCode}.{currentWbsOrder + 1:00}";
                ObjDetail.ACTIVE = true;
                CurrentRepository.Create(ObjDetail);

                UnitOfWork.Commit();
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
                        .Update(x => new T_PS_PROJECT_STRUCT_DRAFT
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

        internal void UpdateTasksTotalDraft(IEnumerable<UpdateTasksTotalDto> data)
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
                        .Update(x => new T_PS_PROJECT_STRUCT_DRAFT
                        {
                            TOTAL = item.Total,
                            UPDATE_BY = currentUser,
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
                        .Update(x => new T_PS_PROJECT_STRUCT_DRAFT
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
            hql.Append(string.Format("DELETE FROM {0} a ", nameof(T_PS_PROJECT_STRUCT_DRAFT)));
            hql.Append($"WHERE a.{nameof(T_PS_PROJECT_STRUCT_DRAFT.PROJECT_ID)} = ? ");
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
                
                if (taskDto.Price < 0)
                {
                    ErrorMessage = "Vui lòng nhập Đơn giá lớn hơn 0!";
                    State = false;
                    return;
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
                currentObj.EXCHANGE_RATE = ObjDetail.EXCHANGE_RATE;
                currentObj.CURRENCY = ObjDetail.CURRENCY;
                currentObj.UPDATE_BY = taskDto.User;

                if (projectStatus == ProjectStatus.KHOI_TAO.GetValue() || projectStatus == ProjectStatus.LAP_KE_HOACH.GetValue())
                {
                    currentObj.PLAN_VOLUME = ObjDetail.QUANTITY;
                }

                this.ObjDetail = this.CurrentRepository.Update(currentObj);

                UnitOfWork.Commit();

            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal List<T_PS_PROJECT_STRUCT_DRAFT> FindChild(Guid parent)
        {
            List<T_PS_PROJECT_STRUCT_DRAFT> lstItem = new List<T_PS_PROJECT_STRUCT_DRAFT>();
            var allChild = UnitOfWork.Repository<ProjectStructDraftRepo>().Queryable().Where(x => x.PARENT_ID == parent).ToList();
            if (allChild.Count > 0)
            {
                foreach (var item in allChild)
                {
                    lstItem.Add(item);
                    var child = UnitOfWork.Repository<ProjectStructDraftRepo>().Queryable().Where(x => x.PARENT_ID == item.ID).ToList();
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

                var taskDelete = UnitOfWork.Repository<ProjectStructDraftRepo>().Queryable().FirstOrDefault(x => x.ID == id);
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

                try
                {
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
                            UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_DRAFT>().Where(x => x.ID == item.ID).Delete();
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
                    ErrorMessage = "Không thể kết nối đến hệ thống SAP hoặc có lỗi khác phát sinh!";
                    State = false;
                    return;
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
                    .Update(x => new T_PS_PROJECT_STRUCT_DRAFT
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
        internal void UpdateDate(Guid projectId, DateTime? fromDate, DateTime? toDate, IList<Guid> structuresId)
        {
            try
            {
                UnitOfWork.Clear();
                UnitOfWork.BeginTransaction();
                var project = SMOUtilities.GetProject(projectId);
                if (fromDate == null && toDate == null)
                {
                    ErrorMessage = $"Vui lòng nhập một trong 2 trường thông tin Từ ngày hoặc Đến ngày!";
                    State = false;
                    return;
                }
                if (fromDate != null && fromDate < project.START_DATE)
                {
                    ErrorMessage = $"Từ ngày không được nhỏ hơn ngày bắt đầu dự án!";
                    State = false;
                    return;
                }
                if (toDate != null && toDate > project.FINISH_DATE)
                {
                    ErrorMessage = $"Đến ngày không được lớn hơn ngày kết thúc dự án!";
                    State = false;
                    return;
                }

                var checkError = "";
                foreach (var item in structuresId)
                {
                    var structItem = UnitOfWork.Repository<ProjectStructDraftRepo>().Get(item);
                    if (fromDate != null)
                    {
                        var checkPlanCost = UnitOfWork.Repository<PlanCostRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.PROJECT_STRUCT_ID == item && x.TimePeriod.FINISH_DATE < toDate).ToList();
                        var sumValue = checkPlanCost.Count() == 0 ? 0 : checkPlanCost.Sum(x => x.VALUE);
                        if (sumValue > 0)
                        {
                            checkError += structItem.GEN_CODE + " ";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(checkError))
                {
                    ErrorMessage = $"Không thể giảm ngày kết thúc của các hạng mục vì đã phát sinh số kế hoạch: " + checkError;
                    State = false;
                    return;
                }

                foreach (var item in structuresId)
                {
                    var structItem = UnitOfWork.Repository<ProjectStructDraftRepo>().Get(item);

                    switch (structItem.TYPE)
                    {
                        case "BOQ":
                            if (fromDate != null)
                            {
                                structItem.START_DATE = (DateTime)fromDate;
                                structItem.Boq.START_DATE = (DateTime)fromDate;
                                structItem.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.UPDATE_DATE = DateTime.Now;
                                structItem.Boq.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.Boq.UPDATE_DATE = DateTime.Now;
                            }
                            if (toDate != null)
                            {
                                structItem.FINISH_DATE = (DateTime)toDate;
                                structItem.Boq.FINISH_DATE = (DateTime)toDate;
                                structItem.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.UPDATE_DATE = DateTime.Now;
                                structItem.Boq.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.Boq.UPDATE_DATE = DateTime.Now;
                            }
                            UnitOfWork.Repository<ProjectStructDraftRepo>().Update(structItem);
                            UnitOfWork.Repository<BoqRepo>().Update(structItem.Boq);
                            break;
                        case "WBS":
                            if (fromDate != null)
                            {
                                structItem.START_DATE = (DateTime)fromDate;
                                structItem.Wbs.START_DATE = (DateTime)fromDate;
                                structItem.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.UPDATE_DATE = DateTime.Now;
                                structItem.Wbs.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.Wbs.UPDATE_DATE = DateTime.Now;
                            }
                            if (toDate != null)
                            {
                                structItem.FINISH_DATE = (DateTime)toDate;
                                structItem.Wbs.FINISH_DATE = (DateTime)toDate;
                                structItem.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.UPDATE_DATE = DateTime.Now;
                                structItem.Wbs.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.Wbs.UPDATE_DATE = DateTime.Now;
                            }
                            UnitOfWork.Repository<ProjectStructDraftRepo>().Update(structItem);
                            UnitOfWork.Repository<WbsRepo>().Update(structItem.Wbs);
                            break;
                        case "ACTIVITY":
                            if (fromDate != null)
                            {
                                structItem.START_DATE = (DateTime)fromDate;
                                structItem.Activity.START_DATE = (DateTime)fromDate;
                                structItem.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.UPDATE_DATE = DateTime.Now;
                                structItem.Activity.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.Activity.UPDATE_DATE = DateTime.Now;
                            }
                            if (toDate != null)
                            {
                                structItem.FINISH_DATE = (DateTime)toDate;
                                structItem.Activity.FINISH_DATE = (DateTime)toDate;
                                structItem.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.UPDATE_DATE = DateTime.Now;
                                structItem.Activity.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                                structItem.Activity.UPDATE_DATE = DateTime.Now;
                            }
                            UnitOfWork.Repository<ProjectStructDraftRepo>().Update(structItem);
                            UnitOfWork.Repository<ActivityRepo>().Update(structItem.Activity);
                            break;
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
        //internal void SaveVersionStructCost(Guid projectId)
        //{
        //    try
        //    {
        //        var allCostStruct = UnitOfWork.Repository<ProjectStructDraftRepo>().Queryable().Where(x => x.PROJECT_ID == projectId && x.TYPE != ProjectEnum.BOQ.ToString());
        //        var checkVersion = UnitOfWork.Repository<ProjectStructVersionRepo>().Queryable().Where(x => x.PROJECT_ID == projectId);
        //        var version = 1;
        //        if (checkVersion.Count() != 0)
        //        {
        //            version = checkVersion.Max(x => x.VERSION) + 1;
        //        }
        //        UnitOfWork.BeginTransaction();
        //        foreach (var item in allCostStruct)
        //        {
        //            UnitOfWork.Repository<ProjectStructVersionRepo>().Create(new T_PS_PROJECT_STRUCT_DRAFT_VERSION
        //            {
        //                PKID = Guid.NewGuid(),
        //                ID = item.ID,
        //                PROJECT_ID = item.PROJECT_ID,
        //                PARENT_ID = item.PARENT_ID,
        //                BOQ_ID = item.BOQ_ID,
        //                WBS_ID = item.WBS_ID,
        //                ACTIVITY_ID = item.ACTIVITY_ID,
        //                TASK_ID = item.TASK_ID,
        //                UNIT_CODE = item.UNIT_CODE,
        //                GEN_CODE = item.GEN_CODE,
        //                TEXT = item.TEXT,
        //                STATUS = item.STATUS,
        //                C_ORDER = item.C_ORDER,
        //                TYPE = item.TYPE,
        //                QUANTITY = item.QUANTITY,
        //                PRICE = item.PRICE,
        //                TOTAL = item.TOTAL,
        //                PLAN_VOLUME = item.PLAN_VOLUME,
        //                START_DATE = item.START_DATE,
        //                FINISH_DATE = item.FINISH_DATE,
        //                IS_CREATE_ON_SAP = item.IS_CREATE_ON_SAP,
        //                VERSION = version
        //            });
        //        }
        //        UnitOfWork.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        UnitOfWork.Rollback();
        //        this.State = false;
        //        this.Exception = ex;
        //    }
        //}

    }
}
