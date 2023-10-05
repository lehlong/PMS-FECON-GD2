using Microsoft.Exchange.WebServices.Data;
using NHibernate.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SharpSapRfc.Plain;
using SharpSapRfc;
using SMO.AppCode.GanttChart;
using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;
using SMO.SAPINT.Class;
using SMO.SAPINT;
using SMO.Service.AD;
using SMO.Service.Class;
using SMO.Service.CM;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using static iTextSharp.text.pdf.AcroFields;
using static SMO.SAPINT.Functions.FunctionPS;

namespace SMO.Service.PS
{
    public class ContractService : GenericService<T_PS_CONTRACT, Repository.Implement.PS.ContractRepo>
    {
        public T_PS_PROJECT_STRUCT ObjFilterProjectStruct { get; set; }
        public List<T_PS_PROJECT_STRUCT> ObjListTaskForAdd { get; set; }
        public ContractService()
        {
            ObjFilterProjectStruct = new T_PS_PROJECT_STRUCT();
            ObjListTaskForAdd = new List<T_PS_PROJECT_STRUCT>();
        }

        public void Create(HttpRequestBase Request, List<string> lstLink)
        {
            try
            {
                if (this.ObjDetail.FINISH_DATE < this.ObjDetail.START_DATE)
                {
                    this.State = false;
                    this.ErrorMessage = "Ngày bắt đầu phải nhỏ hơn ngày kết thúc";
                    return;
                }

                if (this.CheckExist(x => x.CONTRACT_NUMBER == this.ObjDetail.CONTRACT_NUMBER))
                {
                    this.State = false;
                    this.ErrorMessage = $"Số hợp đồng {this.ObjDetail.CONTRACT_NUMBER} đã tồn tại";
                    return;
                }

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
                var currentUser = ProfileUtilities.User?.USER_NAME;

                this.ObjDetail = this.CurrentRepository.Create(this.ObjDetail);
                //if (ObjDetail.IS_SIGN_WITH_CUSTOMER)
                //{
                //    UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, currentUser, "Hợp đồng kinh doanh");
                //}
                UnitOfWork.Commit();
                
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public override void Update()
        {
            try
            {
                var contractInDB = this.UnitOfWork.GetSession().Query<T_PS_CONTRACT>().FirstOrDefault(x => x.ID == this.ObjDetail.ID);
                this.UnitOfWork.Clear();

                UnitOfWork.BeginTransaction();
                if (contractInDB.NGUOI_PHU_TRACH != this.ObjDetail.NGUOI_PHU_TRACH)
                {
                    var lstContractDetail = this.UnitOfWork.GetSession().Query<T_PS_CONTRACT_DETAIL>().Where(x => x.CONTRACT_ID == this.ObjDetail.ID).ToList();
                    var lstTaskId = lstContractDetail.Select(x => x.PROJECT_STRUCT_ID).ToList();
                    var lstStruct = UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => lstTaskId.Contains(x.ID)).ToList();

                    foreach (var task in lstStruct)
                    {
                        if (task.TYPE == "BOQ")
                        {
                            UnitOfWork.GetSession().Query<T_PS_BOQ>().Where(x => x.ID == task.BOQ_ID)
                            .Update(x => new
                            {
                                PEOPLE_RESPONSIBILITY = this.ObjDetail.NGUOI_PHU_TRACH
                            });
                        }

                        if (task.TYPE == "WBS")
                        {
                            UnitOfWork.GetSession().Query<T_PS_WBS>().Where(x => x.ID == task.WBS_ID)
                            .Update(x => new
                            {
                                PEOPLE_RESPONSIBILITY = this.ObjDetail.NGUOI_PHU_TRACH
                            });
                        }

                        if (task.TYPE == "ACTIVITY")
                        {
                            UnitOfWork.GetSession().Query<T_PS_ACTIVITY>().Where(x => x.ID == task.ACTIVITY_ID)
                           .Update(x => new
                           {
                                PEOPLE_RESPONSIBILITY = this.ObjDetail.NGUOI_PHU_TRACH
                           });

                        }
                    }
                }

                this.CurrentRepository.Update(this.ObjDetail);

                //if (contractInDB.IS_SIGN_WITH_CUSTOMER)
                //{
                //    UnitOfWork.Repository<ProjectRepo>().ResetStatus(contractInDB.PROJECT_ID, ProfileUtilities.User.USER_NAME, "Thông tin hợp đồng");
                //}
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void SearchListTaskForAdd()
        {
            var queryStruct = UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>();
            queryStruct = queryStruct.Where(x => x.PROJECT_ID == this.ObjDetail.PROJECT_ID);

            if (!string.IsNullOrWhiteSpace(this.ObjFilterProjectStruct.TEXT))
            {
                queryStruct = queryStruct.Where(x => x.TEXT.Contains(this.ObjFilterProjectStruct.TEXT));
            }

            if (!this.ObjDetail.IS_SIGN_WITH_CUSTOMER)
            {
                queryStruct = queryStruct.Where(x => x.TYPE == "WBS" || x.TYPE == "ACTIVITY");
            }
            else
            {
                queryStruct = queryStruct.Where(x => x.TYPE == "BOQ");
            }

            queryStruct = queryStruct.OrderBy(x => x.C_ORDER);
            ObjListTaskForAdd = queryStruct.ToList();

            var lstContractOfProject = UnitOfWork.GetSession().Query<T_PS_CONTRACT>().Where(x => x.PROJECT_ID == this.ObjDetail.PROJECT_ID).Select(x => x.ID).ToList();
            var lstTaskAssigned = UnitOfWork.GetSession().Query<T_PS_CONTRACT_DETAIL>().Where(x => lstContractOfProject.Contains(x.CONTRACT_ID)).Select(x => x.PROJECT_STRUCT_ID).ToList();

            ObjListTaskForAdd = ObjListTaskForAdd.Where(x => !lstTaskAssigned.Contains(x.ID)).ToList();
        }

        internal void InitTotalValueBeforeTax()
        {
            foreach (var contract in ObjList.Where(x => x.CONTRACT_VALUE == 0))
            {
                contract.CONTRACT_VALUE = UnitOfWork.Repository<ContractDetailRepo>()
                .GetContractDetailsOfContract(contract.ID)
                .Select(x => x.Struct)
                .Where(x => x.TYPE == ProjectEnum.ACTIVITY.ToString())
                .Sum(x => x.QUANTITY * x.PRICE) ?? 0;
            }
        }

        internal void AddTaskToContract(Guid contractId, string strLstSelected)
        {
            this.Get(contractId);
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var lstSplit = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var lstTaskId = new List<Guid>();
                foreach (var item in lstSplit)
                {
                    lstTaskId.Add(Guid.Parse(item));
                }
                var lstStruct = UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => lstTaskId.Contains(x.ID)).ToList();
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<ContractDetailRepo>().Create((from task in lstTaskId
                                                                    select new T_PS_CONTRACT_DETAIL
                                                                    {
                                                                        ID = Guid.NewGuid(),
                                                                        CONTRACT_ID = contractId,
                                                                        PROJECT_STRUCT_ID = task,
                                                                        CREATE_BY = currentUser
                                                                    }).ToList());
                foreach (var task in lstStruct)
                {
                    if (task.TYPE == "BOQ")
                    {
                        UnitOfWork.GetSession().Query<T_PS_BOQ>().Where(x => x.ID == task.BOQ_ID)
                        .Update(x => new
                        {
                            PEOPLE_RESPONSIBILITY = this.ObjDetail.NGUOI_PHU_TRACH
                        });
                    }

                    if (task.TYPE == "WBS")
                    {
                        UnitOfWork.GetSession().Query<T_PS_WBS>().Where(x => x.ID == task.WBS_ID)
                        .Update(x => new
                        {
                            PEOPLE_RESPONSIBILITY = this.ObjDetail.NGUOI_PHU_TRACH
                        });
                    }

                    if (task.TYPE == "ACTIVITY")
                    {
                        UnitOfWork.GetSession().Query<T_PS_ACTIVITY>().Where(x => x.ID == task.ACTIVITY_ID)
                       .Update(x => new
                       {
                           PEOPLE_RESPONSIBILITY = this.ObjDetail.NGUOI_PHU_TRACH
                       });

                    }
                }
                if (ObjDetail.IS_SIGN_WITH_CUSTOMER)
                {
                    UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, currentUser, "Hợp đồng kinh doanh");
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

        internal void UpdateContract(List<UpdateContractModel> models)
        {
            try
            {
                foreach (var model in models)
                {
                    this.Get(model.ContractId);

                    var contractDetailRepo = UnitOfWork.Repository<ContractDetailRepo>();
                    var currentContractDetail = contractDetailRepo.Queryable()
                        .Where(x => x.CONTRACT_ID == model.ContractId && x.PROJECT_STRUCT_ID == model.Data.ProjectStructId)
                        .FirstOrDefault();
                    this.UnitOfWork.Clear();
                    UnitOfWork.BeginTransaction();

                    var currentUser = ProfileUtilities.User?.USER_NAME;
                    var projectStruct = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                        .Where(x => x.ID == model.Data.ProjectStructId)
                        .FirstOrDefault();

                    currentContractDetail.UNIT_CODE = model.Data.UnitCode;
                    currentContractDetail.UNIT_PRICE = model.Data.UnitPrice ?? 0;
                    currentContractDetail.VOLUME = model.Data.Volume ?? 0;
                    currentContractDetail.PROJECT_STRUCT_ID = model.Data.ProjectStructId;
                    currentContractDetail.UPDATE_BY = currentUser;
                    currentContractDetail.ACTIVE = model.Data.IsEnable;
                    contractDetailRepo.Update(currentContractDetail);

                    var status = model.Data.Status == "Chưa bắt đầu" ? "01" : model.Data.Status == "Đang thực hiện" ? "02" : model.Data.Status == "Tạm dừng" ? "03" : model.Data.Status == "Hoàn thành" ? "04" : null;

                    var indexWBS = model.Data.WbsParent.IndexOf("-");
                    var codeWBS = indexWBS == -1 ? model.Data.WbsParent : model.Data.WbsParent.Substring(0, indexWBS - 1);
                    var codeBOQ = "";

                    if (model.Data.BoqRef != null)
                    {
                        var indexBOQ = model.Data.BoqRef.IndexOf("-");
                        codeBOQ = indexBOQ == -1 ? model.Data.BoqRef : model.Data.BoqRef.Substring(0, indexBOQ - 1);
                    }


                    var wbsParent = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == model.ProjectId && x.GEN_CODE == codeWBS && x.TYPE != "PROJECT" && x.TYPE != "BOQ").FirstOrDefault();
                    var boqRef = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == model.ProjectId && x.GEN_CODE == codeBOQ && x.TYPE != "PROJECT" && x.TYPE != "WBS" && x.TYPE != "ACTIVITY").FirstOrDefault();
                    var structId = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.ID == model.Data.ProjectStructId).FirstOrDefault();

                    UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => x.ID == model.Data.ProjectStructId)
                                .Update(x => new T_PS_PROJECT_STRUCT
                                {
                                    STATUS = status,
                                    QUANTITY = model.Data.Volume,
                                    PLAN_VOLUME = model.Data.Volume,
                                    UNIT_CODE = model.Data.UnitCode,
                                    START_DATE = DateTime.Parse(model.Data.StartDate),
                                    FINISH_DATE = DateTime.Parse(model.Data.FinishDate),
                                    PARENT_ID = wbsParent != null ? wbsParent.ID : structId.PARENT_ID,
                                    C_ORDER = wbsParent.ID == structId.PARENT_ID ? structId.C_ORDER : wbsParent.C_ORDER + 0.5,
                                });

                    if (structId.IS_CREATE_ON_SAP)
                    {
                        UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => x.PROJECT_STRUCT_ID == model.Data.ProjectStructId).Delete();
                        UnitOfWork.GetSession().Save(new T_PS_PROJECT_STRUCT_SAP()
                        {
                            ID = Guid.NewGuid(),
                            PROJECT_STRUCT_ID = model.Data.ProjectStructId,
                            PROJECT_ID = model.ProjectId,
                            ACTION = "U"
                        });
                    }

                    if (boqRef != null)
                    {
                        if (structId.TYPE == "ACTIVITY")
                        {
                            UnitOfWork.GetSession().Query<T_PS_ACTIVITY>().Where(x => x.ID == structId.Activity.ID)
                                                                            .Update(x => new T_PS_ACTIVITY
                                                                            {
                                                                                BOQ_REFRENCE_ID = boqRef.ID
                                                                            });
                        }
                        if (structId.TYPE == "WBS")
                        {
                            UnitOfWork.GetSession().Query<T_PS_WBS>().Where(x => x.ID == structId.Wbs.ID)
                                                                            .Update(x => new T_PS_WBS
                                                                            {
                                                                                BOQ_REFRENCE_ID = boqRef.ID
                                                                            });
                        }
                    }

                    if (ObjDetail.IS_SIGN_WITH_CUSTOMER && currentContractDetail.STATUS == status)
                    {
                        UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, currentUser, "Hợp đồng kinh doanh");
                    }

                    UnitOfWork.Commit();
                }
                

                var allProjectStruct = UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.PROJECT_ID == models.FirstOrDefault().ProjectId && x.TYPE != "BOQ").OrderBy(x => x.C_ORDER).Select(y => y.ID).ToArray();               
                UnitOfWork.BeginTransaction();
                for (int i = 0; i < allProjectStruct.Length; i++)
                {
                    UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => x.ID == allProjectStruct[i])
                            .Update(x => new T_PS_PROJECT_STRUCT
                            {
                                UPDATE_BY = ProfileUtilities.User.USER_NAME,
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

        internal bool CheckCanEditContractCode(Guid id)
        {
            return !UnitOfWork.Repository<ContractDetailRepo>().Queryable().Any(c => c.CONTRACT_ID == id);
        }

        internal void RemoveTasksFromContract(Guid contractId, IList<Guid> structureIds)
        {
            try
            {
                Get(contractId);
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var structureInWork = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                    .Any(x => structureIds.Contains(x.PROJECT_STRUCT_ID));
                var structureInAccept = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
                    .Any(x => structureIds.Contains(x.PROJECT_STRUCT_ID));
                if (structureInWork || structureInAccept)
                {
                    ErrorMessage = "Đã phát sinh dữ liệu thực tế, không thể xóa hạng mục khỏi hợp đồng";
                    State = false;
                    return;
                }
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<ContractDetailRepo>()
                    .Queryable()
                    .Where(x => x.CONTRACT_ID == contractId && structureIds.Contains(x.PROJECT_STRUCT_ID))
                    .Delete();

                if (ObjDetail.IS_SIGN_WITH_CUSTOMER)
                {
                    UnitOfWork.Repository<ProjectRepo>().ResetStatus(ObjDetail.PROJECT_ID, currentUser, "Hợp đồng kinh doanh");
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

        internal void RemoveTasksFromContractAndTree(Guid contractId, IList<Guid> structureIds)
        {
            try
            {
                var structureInWork = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                    .Any(x => structureIds.Contains(x.PROJECT_STRUCT_ID));
                var structureInAccept = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
                    .Any(x => structureIds.Contains(x.PROJECT_STRUCT_ID));

                if (structureInWork || structureInAccept)
                {
                    if(UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                    .Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Sum(x => x.VALUE) >0 || UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
                    .Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Sum(x => x.VALUE) > 0)
                    {
                        ErrorMessage = "Đã phát sinh dữ liệu thực tế, không thể xóa hạng mục khỏi hợp đồng và cây cấu trúc";
                        State = false;
                        return;
                    }
                }

                var lstStructDelete = new List<T_PS_PROJECT_STRUCT>();
                foreach(var item in structureIds)
                {
                    var structItem = UnitOfWork.Repository<ProjectStructRepo>().Queryable().FirstOrDefault(x => x.ID == item);
                    if(structItem != null)
                    {
                        lstStructDelete.Add(structItem);
                    }
                }
                var checkSAP = false;
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();

                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                            systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new Update_Project_Function();
                    var structIdOfProject = lstStructDelete.FirstOrDefault().Project.ID;

                    var project = new ZST_BAPIPROJ()
                    {
                        PROJECT_CODE = lstStructDelete.FirstOrDefault().Project.CODE,
                        COMPANY_CODE = lstStructDelete.FirstOrDefault().Project.DonVi.COMPANY_CODE,
                        COST_CENTER_CODE = lstStructDelete.FirstOrDefault().Project.DonVi.COST_CENTER_CODE,
                        PROJECT_NAME = lstStructDelete.FirstOrDefault().Project.NAME,
                        FINISH_DATE = lstStructDelete.FirstOrDefault().Project.FINISH_DATE,
                        START_DATE = lstStructDelete.FirstOrDefault().Project.START_DATE,
                    };

                    var lstWbs = new List<ZST_BAPIWBS>();
                    foreach (var item in lstStructDelete.Where(x => x.TYPE == "WBS").ToList())
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
                    foreach (var item in lstStructDelete.Where(x => x.TYPE == "ACTIVITY").ToList())
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

                if (checkSAP)
                {
                    ErrorMessage = "Đã phát sinh dữ liệu thực tế, không thể xóa hạng mục khỏi hợp đồng và cây cấu trúc";
                    State = false;
                    return;
                }
                else
                {
                    UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT_SAP>().Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Delete();
                    UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(x => structureIds.Contains(x.ID)).Delete();
                    UnitOfWork.GetSession().Query<T_PS_CONTRACT_DETAIL>().Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Delete();
                    // delete plan cost
                    UnitOfWork.GetSession().Query<T_PS_PLAN_COST>().Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Delete();
                    // delete plan progress
                    UnitOfWork.GetSession().Query<T_PS_PLAN_PROGRESS>().Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Delete();
                    // delete plan quantity
                    UnitOfWork.GetSession().Query<T_PS_PLAN_QUANTITY>().Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Delete();
                    //delete volume work
                    UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK_DETAIL>().Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Delete();
                    //delete volume accept
                    UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT_DETAIL>().Where(x => structureIds.Contains(x.PROJECT_STRUCT_ID)).Delete();
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

        internal IList<TreeContractProjectStruct> BuildTreeContractProjectStructs()
        {
            var query = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID);
            if (ObjDetail.IS_SIGN_WITH_CUSTOMER)
            {
                query = query.Where(x => x.TYPE == ProjectEnum.PROJECT.ToString() || x.TYPE == ProjectEnum.BOQ.ToString());
            }
            else
            {
                query = query.Where(x => x.TYPE == ProjectEnum.PROJECT.ToString()
                || x.TYPE == ProjectEnum.WBS.ToString()
                || x.TYPE == ProjectEnum.ACTIVITY.ToString());
            }
            var projectStructs = query.OrderBy(x => x.C_ORDER).ToList();

            var lstTaskAssign = this.ObjDetail.Details.Select(x => x.PROJECT_STRUCT_ID).ToList();
            projectStructs = projectStructs.Where(x => lstTaskAssign.Contains(x.ID)).ToList();
            return BuildTree(projectStructs);
        }

        private IList<TreeContractProjectStruct> BuildTree(List<T_PS_PROJECT_STRUCT> projectStructs)
        {
            var isAssignedCustomer = ObjDetail.IS_SIGN_WITH_CUSTOMER;
            var assignedContractActivities = UnitOfWork.Repository<ContractDetailRepo>().Queryable()
                .Where(x => x.Contract.PROJECT_ID == ObjDetail.PROJECT_ID).ToList();
            var lstUserOfProject = this.UnitOfWork.Repository<ProjectResourceRepo>().Queryable()
                .Where(x => x.PROJECT_ID == this.ObjDetail.PROJECT_ID)
                .OrderBy(x => x.USER_NAME)
                .ToList();

            foreach (var task in projectStructs)
            {
                var userIdPhuTrach = "";
                if (task.TYPE == "WBS")
                {
                    userIdPhuTrach = task.Wbs.PEOPLE_RESPONSIBILITY;
                }

                if (task.TYPE == "BOQ")
                {
                    userIdPhuTrach = task.Boq.PEOPLE_RESPONSIBILITY;
                }

                if (task.TYPE == "ACTIVITY")
                {
                    userIdPhuTrach = task.Activity.PEOPLE_RESPONSIBILITY;
                }
                var lstId = new List<Guid>();
                if (!string.IsNullOrEmpty(userIdPhuTrach))
                {
                    foreach (var item in userIdPhuTrach.Split(','))
                    {
                        lstId.Add(Guid.Parse(item));
                    }
                }
                task.NguoiPhuTrach = String.Join(", ", lstUserOfProject.Where(x => lstId.Contains(x.ID)).Select(x => x.User.FULL_NAME).ToList());
            }
            return (from projectStruct in projectStructs
                    let contractDetail = ObjDetail.Details.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                    select new TreeContractProjectStruct
                    {
                        Id = projectStruct.ID,
                        Parent = projectStruct.PARENT_ID,
                        Code = projectStruct.GEN_CODE,
                        Name = projectStruct.TEXT,
                        UnitCode = projectStruct.UNIT_CODE,
                        UnitName = projectStruct.Unit?.NAME,
                        StartDate = projectStruct.START_DATE.ToString("dd/MM/yyyy"),
                        FinishDate = projectStruct.FINISH_DATE.ToString("dd/MM/yyyy"),
                        WbsParent = projectStruct.Parent?.GEN_CODE,
                        BoqRef = projectStruct.TYPE == "ACTIVITY" ? projectStruct.Activity?.ReferenceBoq?.GEN_CODE : projectStruct.TYPE == "WBS" ? projectStruct.Wbs?.ReferenceBoq?.GEN_CODE : null,
                        UnitPrice = ObjDetail.IS_SIGN_WITH_CUSTOMER ? projectStruct.PRICE : contractDetail.UNIT_PRICE,
                        Volume = projectStruct.QUANTITY,
                        Total = ObjDetail.IS_SIGN_WITH_CUSTOMER ? projectStruct.TOTAL : contractDetail.UNIT_PRICE * projectStruct.QUANTITY,
                        Type = (ProjectEnum)Enum.Parse(typeof(ProjectEnum), projectStruct.TYPE),
                        NguoiPhuTrach = projectStruct.NguoiPhuTrach,
                        Status = projectStruct.STATUS
                    }).ToList();
        }

        public void DeleteContract(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.BeginTransaction();
                foreach (var item in lstId)
                {
                    var detailContract = UnitOfWork.Repository<ContractDetailRepo>().Queryable().Where(x => x.CONTRACT_ID.ToString() == item).ToList();
                    var valueWorkVolume = (from x in detailContract
                                          join y in UnitOfWork.Repository<VolumeWorkDetailRepo>().GetAll() on x.PROJECT_STRUCT_ID equals y.PROJECT_STRUCT_ID
                                          select new { contractDetail = x, volumeWork = y }).ToList();
                  
                    if(valueWorkVolume.FirstOrDefault() != null && valueWorkVolume.Sum(x => x.volumeWork.VALUE) > 0)
                    {
                        ErrorMessage = "Đã phát sinh dữ liệu thực hiện, không thể xóa hợp đồng";
                        State = false;
                        return;
                    }
                    else
                    {
                        UnitOfWork.GetSession().Query<T_PS_CONTRACT>().Where(x => x.ID.ToString() == item).Delete();
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
    }
}
