using iTextSharp.text;
using NHibernate.Linq;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

using SharpSapRfc;
using SharpSapRfc.Plain;

using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.PS;
using SMO.SAPINT;
using SMO.SAPINT.Class;
using SMO.Service;
using SMO.Service.AD;
using SMO.Service.Common;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using static SMO.SAPINT.Functions.FunctionPS;

namespace SMO.Service.PS
{
    public class VolumeWorkService : BaseProjectVolumeService<T_PS_VOLUME_WORK, VolumeWorkRepo>
    {
        #region Header Text
        internal string GetTextDuAn(string text)
        {
                return $"Dự án: {text}";
        }
        internal string GetTextNhaThau(string text)
        {
            return $"Nhà thầu: {text}";
        }
        internal string GetTextKhachHang(string text)
        {
            return $"Khách hàng: {text}";
        }
        internal string GetTextTuNgay(string text)
        {
            return $"Từ ngày: {text}";
        }
        internal string GetTextDenNgay(string text)
        {
            return $"Đến ngày: {text}";
        }


        #endregion

        private ICellStyle GetCellStyleNumber(IWorkbook templateWorkbook)
        {
            ICellStyle styleCellNumber = templateWorkbook.CreateCellStyle();
            styleCellNumber.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0");
            return styleCellNumber;
        }
        private ICellStyle GetCellStyleNumberDecimal(IWorkbook templateWorkbook)
        {
            ICellStyle styleCellNumber = templateWorkbook.CreateCellStyle();
            styleCellNumber.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0.0000000000");
            return styleCellNumber;
        }
        private ICellStyle GetCellStylePercentage(IWorkbook templateWorkbook)
        {
            ICellStyle styleCellPercentage = templateWorkbook.CreateCellStyle();
            styleCellPercentage.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0.00%");
            return styleCellPercentage;
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
                        
                        if (j >= config.StartColumnNumber && (!config.EndColumnNumber.HasValue || j <= config.EndColumnNumber.Value))
                        {
                            double value = 0;
                            var canParseNumber = double.TryParse(valueStr, out value);
                            var isHeaderVolume = header.Any(x => x.ToList()[j].Contains("KL") || x.ToList()[j].Contains("LK") || x.ToList()[j].ToLower().Contains("khối lượng"));
                            rowCur.Cells[j].CellStyle = config.ColumnsPercentage.Contains(j)
                                || (isRowPercentageUnit && config.ColumnsVolume.Contains(j)) ?
                                styleCellPercentage :
                                isHeaderVolume ? styleCellNumberDecimal : styleCellNumber;
                            rowCur.Cells[j].SetCellValue(canParseNumber ? value : 0);

                            if (dataRow.ElementAt(4) == "%" && isHeaderVolume)
                            {
                                rowCur.Cells[j].CellStyle = styleCellPercentage;
                                rowCur.Cells[j].SetCellValue(value);
                            }
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

        internal void DeleteOnSAP(Guid id)
        {
            this.Get(id);
            if (string.IsNullOrEmpty(this.ObjDetail.SAP_DOCID))
            {
                return;
            }

            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                                systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new Delete_ThucHien_Function();
                    functionSAP.Parameters = new
                    {
                        DOC_NUM = this.ObjDetail.SAP_DOCID
                    };

                    conn.ExecuteFunction(functionSAP);
                    var output = functionSAP.Result.GetTable<BAPIRET2>("TB_RETURN");
                    this.State = true;
                    if (output.Count(x => x.TYPE == "E") > 0)
                    {
                        this.State = false;
                        this.ErrorMessage = "Xóa chứng từ trên SAP không thành công !" + string.Join("<br/>", output.Where(x => x.TYPE == "E").ToList().Select(x => x.MESSAGE));
                    }
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Xóa chứng từ trên SAP không thành công !";
                this.Exception = ex;
            }
        }

        internal string SynToSAP(Guid id)
        {
            var docNum = "";
            this.Get(id);
            var projectStructureIdsInList = this.ObjDetail.Details.Where(x => x.VALUE != 0).Select(x => x.PROJECT_STRUCT_ID).ToList();
            var lstProjectStruct = UnitOfWork.GetSession().Query<T_PS_PROJECT_STRUCT>().Where(
                        x => projectStructureIdsInList.Contains(x.ID)).ToList();
            var project = UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == this.ObjDetail.PROJECT_ID);
            try
            {
                var systemConfig = new SystemConfigService();
                systemConfig.GetConfig();
                using (SapRfcConnection conn = new PlainSapRfcConnection(SAPDestitination.SapDestinationName,
                                systemConfig.ObjDetail.SAP_USER_NAME, systemConfig.ObjDetail.SAP_PASSWORD))
                {
                    var functionSAP = new Create_ThucHien_Function();
                    var lstThucHien = new List<ZST_ITHUCHIEN>();
                    string headerText = $"{project.CODE} - {project.NAME}: Khối lượng thầu phụ từ ngày {this.ObjDetail.FROM_DATE.Value.ToString(Global.DateToStringFormat)} đến ngày {this.ObjDetail.TO_DATE.Value.ToString(Global.DateToStringFormat)}";
                    if (this.ObjDetail.IS_CUSTOMER)
                    {
                        headerText = $"{project.CODE} - {project.NAME}: Giá trị sản lượng từ ngày {this.ObjDetail.FROM_DATE.Value.ToString(Global.DateToStringFormat)} đến ngày {this.ObjDetail.TO_DATE.Value.ToString(Global.DateToStringFormat)}";
                    }

                    if (!this.ObjDetail.IS_CUSTOMER)
                    {
                        foreach (var item in lstProjectStruct)
                        {
                            var workDetail = this.ObjDetail.Details.First(x => x.PROJECT_STRUCT_ID == item.ID);
                            var contractDetail = this.UnitOfWork.GetSession().Query<T_PS_CONTRACT_DETAIL>().FirstOrDefault(x => x.PROJECT_STRUCT_ID == item.ID);
                            lstThucHien.Add(new ZST_ITHUCHIEN()
                            {
                                ZDATE = this.ObjDetail.TO_DATE,
                                H_TEXT = headerText,
                                ZPS01 = item.TYPE == "ACTIVITY" ? "X" : "",
                                ZPS03 = item.TYPE == "WBS" ? "X" : "",
                                RECOPERATN = item.TYPE == "ACTIVITY" ? item.Activity.CODE : "",
                                REC_WBS_EL = item.TYPE == "WBS" ? item.Wbs.CODE : "",
                                STATKEYFIG = item.Unit?.SKF,
                                STAT_QTY = Math.Round(item.UNIT_CODE == "%" ? workDetail.VALUE * 100 : workDetail.VALUE, 3)
                            });
                        }
                    }
                    else
                    {
                        decimal sum = 0;
                        foreach (var item in lstProjectStruct)
                        {
                            var workDetail = this.ObjDetail.Details.First(x => x.PROJECT_STRUCT_ID == item.ID);
                            sum += (item.PRICE ?? 0) * workDetail.VALUE;
                        }
                        lstThucHien.Add(new ZST_ITHUCHIEN()
                        {
                            ZDATE = this.ObjDetail.TO_DATE,
                            H_TEXT = headerText,
                            ZPS01 = "",
                            ZPS03 = "X",
                            RECOPERATN = "",
                            REC_WBS_EL = project.CODE,
                            STATKEYFIG = "PS0042",
                            STAT_QTY = Math.Round(sum, 3)
                        });
                    }


                    functionSAP.Parameters = new
                    {
                        PROJECT = project.CODE,
                        IMPORT = lstThucHien
                    };

                    conn.ExecuteFunction(functionSAP);
                    var output = functionSAP.Result.GetOutput<BAPIRET2>("RETURN");
                    this.State = true;
                    if (output.TYPE == "E")
                    {
                        this.State = false;
                        this.ErrorMessage = "Đẩy thông tin lên SAP không thành công !" + output.MESSAGE;
                    }
                    else
                    {
                        docNum = functionSAP.Result.GetOutput<string>("DOCNUM");
                    }
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.ErrorMessage = "Đẩy thông tin lên SAP không thành công !";
                this.Exception = ex;
            }
            return docNum;
        }

        internal IEnumerable<T_PS_VOLUME_WORK_DETAIL> GetAllVolumeWorkDetail()
        {
            return UnitOfWork.Repository<VolumeWorkDetailRepo>().GetAll().ToList();
        }
        public override ProjectWorkVolumeStatus UpdateStatus(UpdateStatusVolumeModel model, bool isAccept)
        {
            var currentObj = CurrentRepository.Get(model.Id);
            CurrentRepository.Detach(currentObj);
            var project = UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == currentObj.PROJECT_ID);
            var docNum = "";
            var statusFromAction = GetStatusFromAction(model.Action.GetEnum<ProjectWorkVolumeAction>());
            if (statusFromAction.GetValue() == ProjectWorkVolumeStatus.XAC_NHAN.GetValue() ||
                statusFromAction.GetValue() == ProjectWorkVolumeStatus.PHE_DUYET.GetValue()
                )
            {
                var projectStructureIds = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                    .Where(x => x.HEADER_ID == model.Id).Select(x => x.PROJECT_STRUCT_ID)
                    .ToList();
                
                ValidateDupplicate(projectStructureIds, currentObj.FROM_DATE, currentObj.TO_DATE, currentObj.PROJECT_ID, currentObj.ID,
                    currentObj.IS_CUSTOMER, currentObj.VENDOR_CODE);
                if (!this.State)
                {
                    return ProjectWorkVolumeStatus.CHO_XAC_NHAN;
                }
            }
            //if (statusFromAction.GetValue() == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
            //{
            //    var lstCode = new List<string>() { "C.0397", "C.0390", "C.0382", "C.0383", "C.0387", "C.0380", "C.0389", "C.0395", "C.0384" };
            //    if (!lstCode.Contains(project.CODE))
            //    {
            //        docNum = this.SynToSAP(model.Id); 
            //    }

            //    if (this.State)
            //    {
            //        this.ObjDetail.SAP_DOCID = docNum;
            //        model.SAP_DOCID = docNum;
            //    }
            //}

            //if (model.Action == ProjectWorkVolumeAction.HUY_PHE_DUYET.GetValue())
            //{
            //    this.DeleteOnSAP(model.Id);
            //    if (this.State)
            //    {
            //        this.ObjDetail.SAP_DOCID = "";
            //        model.SAP_DOCID = "";
            //    }
            //}

            return base.UpdateStatus(model, isAccept);
        }

        public IList<VolumeWorkViewModel> GetPlanProgresses()
        {
            bool isNewObj = ObjDetail.ID == Guid.Empty;
            if (!isNewObj)
            {
                Get(ObjDetail.ID);
            }
            var projectTimes = GetProjectTime();
            var inProjectTimes = isNewObj ? new List<Guid>() : projectTimes.Where(projectTime =>
            {
                var isStartDateInPeriod = ObjDetail.FROM_DATE >= projectTime.START_DATE && ObjDetail.FROM_DATE <= projectTime.FINISH_DATE;
                var isEndDateInPeriod = ObjDetail.TO_DATE >= projectTime.START_DATE && ObjDetail.TO_DATE <= projectTime.FINISH_DATE;
                var isPeriodInsideRange = ObjDetail.TO_DATE >= projectTime.FINISH_DATE && ObjDetail.FROM_DATE <= projectTime.START_DATE;

                return isEndDateInPeriod || isStartDateInPeriod || isPeriodInsideRange;
            }).Select(x => x.ID);
            var lastProjectTimeOrder = projectTimes.Where(x => x.START_DATE < ObjDetail.TO_DATE && x.FINISH_DATE >= ObjDetail.FROM_DATE)
                .Select(x => x.C_ORDER)
                .FirstOrDefault();
            var previousProjectTimes = isNewObj ? new List<Guid>() : projectTimes.Where(projectTime => lastProjectTimeOrder >= projectTime.C_ORDER)
                .Select(x => x.ID);
            var allProjectWorkVolumesPheDuyet = CurrentRepository.Queryable()
                .Where(x => x.TO_DATE <= ObjDetail.TO_DATE && x.PROJECT_ID == ObjDetail.PROJECT_ID && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                .SelectMany(x => x.Details)
                .ToList();
            var allProjectWorkVolumes = CurrentRepository.Queryable()
                .Where(x => x.TO_DATE == ObjDetail.TO_DATE && x.PROJECT_ID == ObjDetail.PROJECT_ID && x.ID == ObjDetail.ID)
                .SelectMany(x => x.Details)
                .ToList();
            var lstProjectWorkVolumes = CurrentRepository.Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID && x.STATUS != ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue() && x.STATUS != ProjectWorkVolumeStatus.TU_CHOI.GetValue())
                .SelectMany(x => x.Details)
                .ToList();
            var contracts = UnitOfWork.Repository<ContractRepo>().Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID
                && x.IS_SIGN_WITH_CUSTOMER == ObjDetail.IS_CUSTOMER
                && x.VENDOR_CODE == ObjDetail.VENDOR_CODE)
                .ToList();
            var allPlanVolumes = UnitOfWork.Repository<PlanCostRepo>().Queryable()
                .Where(x => x.IS_CUSTOMER == ObjDetail.IS_CUSTOMER
                && x.PROJECT_ID == ObjDetail.PROJECT_ID
                && previousProjectTimes.Contains(x.TIME_PERIOD_ID))
                .ToList();
            var projectStructures = ObjDetail.IS_CUSTOMER ?
                UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID).ToList()
                : new List<T_PS_PROJECT_STRUCT>();
            return (from contract in contracts
                    from contractDetail in contract.Details
                    let accumulatedPlanVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID).Sum(x => x.VALUE)
                    let planVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID
                    && inProjectTimes.Contains(x.TIME_PERIOD_ID))
                    .Sum(x => x.VALUE)
                    let designVolume = projectStructures.Where(x => x.ID == contractDetail.PROJECT_STRUCT_ID).FirstOrDefault()?.PLAN_VOLUME
                    let volumeWorkDetail = ObjDetail.Details.FirstOrDefault(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let workVolumes = lstProjectWorkVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let projectWorkVolumes = allProjectWorkVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let projectWorkVolumesPheDuyet = allProjectWorkVolumesPheDuyet.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let unitPriceContract = volumeWorkDetail?.PRICE ?? (ObjDetail.IS_CUSTOMER ? projectStructures.FirstOrDefault(x => x.ID == contractDetail.PROJECT_STRUCT_ID)?.PRICE : contractDetail.UNIT_PRICE)
                    //let unitPriceContract = contractDetail.UNIT_PRICE
                    select ConvertToViewModel(workVolumes, projectWorkVolumes, projectWorkVolumesPheDuyet, contractDetail, contract, planVolume, accumulatedPlanVolume, unitPriceContract, designVolume))
                    .OrderBy(x => x.Order)
                    .Distinct()
                    .ToList();
        }


        internal VolumeWorkViewModel ConvertToViewModel(IEnumerable<T_PS_VOLUME_WORK_DETAIL> workVolumes,
            IEnumerable<T_PS_VOLUME_WORK_DETAIL> projectWorkVolumes,
            IEnumerable<T_PS_VOLUME_WORK_DETAIL> projectWorkVolumesPheDuyet,
                                                        T_PS_CONTRACT_DETAIL contractDetail,
                                                        T_PS_CONTRACT contract,
                                                        decimal planVolume,
                                                        decimal accumulatedPlanVolume,
                                                        decimal? unitPriceContract,
                                                        decimal? designVolume)
        {
            var currentValue = ObjDetail.Details.FirstOrDefault(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID);
            var structType = contractDetail.Struct.TYPE;
            DateTime? actualFinishedDate = null;
            switch (structType)
            {
                case "WBS":
                    actualFinishedDate = contractDetail.Struct.Wbs.ACTUAL_FINISH_DATE;
                    break;
                case "BOQ":
                    actualFinishedDate = contractDetail.Struct.Boq.ACTUAL_FINISH_DATE;
                    break;
                case "ACTIVITY":
                    actualFinishedDate = contractDetail.Struct.Activity.ACTUAL_FINISH_DATE;
                    break;
                default:
                    break;
            }
            //if (ObjDetail.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())

            return new VolumeWorkViewModel
            {
                DbId = currentValue?.ID,
                ProjectStructureId = contractDetail.PROJECT_STRUCT_ID,
                ParentId = contractDetail.Struct.PARENT_ID,
                Notes = currentValue?.NOTES,
                ProjectId = ObjDetail.PROJECT_ID,
                AcceptVolume = currentValue?.CONFIRM_VALUE,
                ReferenceFileId = currentValue?.REFERENCE_FILE_ID,
                ContractName = contract.NAME,
                ContractCode = contract.CONTRACT_NUMBER,
                ContractDetailId = contractDetail.ID,
                //ContractVolume = ObjDetail.IS_CUSTOMER ? contractDetail.Struct.QUANTITY : contractDetail.VOLUME,
                ContractVolume = contractDetail.Struct.QUANTITY,
                ProjectStructureType = contractDetail.Struct.TYPE,
                ProjectStructureCode = contractDetail.Struct.GEN_CODE,
                ProjectStructureName = contractDetail.Struct.TEXT,
                UnitCode = contractDetail.Struct.UNIT_CODE,
                PlanVolume = planVolume,
                AccumulatedPlanVolume = accumulatedPlanVolume,
                WorkVolume = currentValue?.VALUE,
                DesignVolume = designVolume,
                Price = unitPriceContract,
                Order = contractDetail.Struct.C_ORDER,
                AccumulatedVolume =(ObjDetail.STATUS == ProjectWorkVolumeStatus.TU_CHOI.GetValue() || ObjDetail.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue()) ? projectWorkVolumesPheDuyet.Sum(x => x.VALUE) : projectWorkVolumesPheDuyet.Sum(x => x.VALUE) + currentValue?.VALUE,
                TotalAccumulatedVolume = workVolumes.Sum(x => x.VALUE),
                AccumulateAcceptedVolume = projectWorkVolumes.Sum(x => x.CONFIRM_VALUE),
                FinishedDate = actualFinishedDate,
                PercentageDone = contractDetail.VOLUME == 0 ? 0 : (currentValue?.VALUE / contractDetail.VOLUME * 100),
                WorkStatus = contractDetail.Struct.STATUS.GetEnum<ProjectStructStatus>().GetName(),
                WorkStatusCode = contractDetail.Struct.STATUS ?? ProjectStructStatus.KHOI_TAO.GetValue(),
                UserUpdated = currentValue?.UPDATE_BY ?? currentValue?.CREATE_BY
            };
        }

        private void ValidateDupplicate(IEnumerable<Guid> projectStructureIds, DateTime? startDate, DateTime? finishDate, Guid projectId, Guid headerId, bool isCustomer, string partnerCode)
        {
            var queryHeader = CurrentRepository.Queryable().Where(x => x.PROJECT_ID == projectId
                && x.STATUS != ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue() && x.STATUS != ProjectWorkVolumeStatus.TU_CHOI.GetValue());
            if (headerId != Guid.Empty)
            {
                queryHeader = queryHeader.Where(x => x.ID != headerId);
            }
            if (!isCustomer)
            {
                queryHeader = queryHeader.Where(x => x.VENDOR_CODE == partnerCode);
            }
            var lstHeader = queryHeader.ToList();
            var lstHeaderIdSamePeriod = lstHeader.Where(x =>
            {
                var isStartDateInPeriod = startDate >= x.FROM_DATE && startDate <= x.TO_DATE;
                var isEndDateInPeriod = finishDate >= x.FROM_DATE && finishDate <= x.TO_DATE;
                var isPeriodInsideRange = finishDate >= x.TO_DATE && startDate <= x.FROM_DATE;

                return isEndDateInPeriod || isStartDateInPeriod || isPeriodInsideRange;
            }).Select(x => x.ID).ToList();

            if (lstHeaderIdSamePeriod.Count > 0)
            {
                var structureIdExistInPeriod = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                .Where(x => projectStructureIds.Contains(x.PROJECT_STRUCT_ID) && lstHeaderIdSamePeriod.Contains(x.HEADER_ID))
                .Select(x => x.PROJECT_STRUCT_ID)
                .ToList();

                if (structureIdExistInPeriod.Count > 0)
                {
                    var structureCodes = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                        .Where(x => structureIdExistInPeriod.Contains(x.ID))
                        .Select(x => x.GEN_CODE)
                        .ToList();
                    this.State = false;
                    this.ErrorMessage = $"Đợt nhập có hạng mục {string.Join(", ", structureCodes)} trùng đợt nhập khối lượng " +
                        $"từ {startDate.Value.ToString(Global.DateToStringFormat)} - đến {finishDate.Value.ToString(Global.DateToStringFormat)}";
                    return;
                }
            }
        }

        internal void Validate(AcceptTimesModel model)
        {
            if (model.Id != Guid.Empty)
            {
                var currentObj = CurrentRepository.Get(model.Id);
                if (!new string[] {@ProjectWorkVolumeStatus.KHOI_TAO.GetValue(),
                    @ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue(),
                    @ProjectWorkVolumeStatus.TU_CHOI.GetValue() }.Contains(currentObj.STATUS))
                {
                    this.State = false;
                    this.ErrorMessage = "Trạng thái của đợt nhập đã bị thay đổi trước đó. Hãy thoát khỏi màn hình rồi vào lại.";
                    return;
                }
            }

            var project = this.UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == model.ProjectId);
            ObjDetail.PROJECT_ID = model.ProjectId;
            var timePeriods = GetProjectTime();
            var currentTimePeriod = timePeriods.FirstOrDefault(x => x.START_DATE <= model.FinishDate && x.FINISH_DATE >= model.FinishDate);

            if (currentTimePeriod == null)
            {
                State = false;
                ErrorMessage = "Thời gian nằm ngoài khoảng thời gian của dự án.";
                return;
            }

            if (model.FinishDate < model.StartDate)
            {
                State = false;
                ErrorMessage = "Thời gian bắt đầu phải nhỏ hoặc bằng thời gian kết thúc.";
                return;
            }

            model.Details = model.Details.Where(x => x.WorkVolume != 0).ToList();
            if (model.Details.Count() == 0)
            {
                this.State = false;
                this.ErrorMessage = "Hãy nhập ít nhất một hạng mục có giá trị khác 0!";
                return;
            }
            var projectStructureIds = model.Details.Select(x => x.ProjectStructureId).ToList();
            ValidateDupplicate(projectStructureIds, model.StartDate, model.FinishDate, model.ProjectId, model.Id, model.IsCustomer, model.PartnerCode);
        }

        internal Guid AddUpdateTimes(AcceptTimesModel model, string isSent, string yKien)
        {
            var lstUserNotify = new List<string>();
            Validate(model);
            if (!this.State)
            {
                return Guid.Empty;
            }

            UnitOfWork.Clear();
            var project = this.UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == model.ProjectId);
            var allUpdateTimes = CurrentRepository.Queryable()
                .Where(x =>
                x.VENDOR_CODE == (model.IsCustomer ? null : model.PartnerCode)
                && x.PROJECT_ID == model.ProjectId
                && x.IS_CUSTOMER == model.IsCustomer)
                .ToList();
            var currentUpdateTimes = allUpdateTimes.Count == 0 ? 0 : allUpdateTimes.Max(x => x.UPDATE_TIMES);
            var timePeriods = GetProjectTime();
            var currentTimePeriod = timePeriods.FirstOrDefault(x => x.START_DATE <= model.FinishDate && x.FINISH_DATE >= model.FinishDate);

            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var headerId = Guid.NewGuid();
                UnitOfWork.BeginTransaction();
                if (model.Id != null && model.Id != Guid.Empty)
                {
                    headerId = model.Id;
                    if (isSent == "1")
                    {
                        this.UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK>().Where(x => x.ID == model.Id).Update(x => new T_PS_VOLUME_WORK()
                        {
                            FROM_DATE = model.StartDate,
                            TO_DATE = model.FinishDate,
                            STATUS = ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue(),
                            UPDATE_BY = currentUser
                        });

                        UnitOfWork.Repository<VolumeProgressHistoryRepo>()
                    .Create(new T_PS_VOLUME_PROGRESS_HISTORY
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = model.ProjectId,
                        ACTOR = currentUser,
                        CREATE_BY = currentUser,
                        ACTION = ProjectWorkVolumeAction.GUI.GetValue(),
                        DES_STATUS = ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue(),
                        PRE_STATUS = ProjectWorkVolumeStatus.KHOI_TAO.GetValue(),
                        NOTE = yKien,
                        RESOURCE_ID = headerId,
                        IS_CUSTOMER = model.IsCustomer,
                        IS_ACCEPT = false
                    });
                    }
                    else
                    {
                        this.UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK>().Where(x => x.ID == model.Id).Update(x => new T_PS_VOLUME_WORK()
                        {
                            FROM_DATE = model.StartDate,
                            TO_DATE = model.FinishDate,
                            UPDATE_BY = currentUser
                        });
                    }
                }
                else
                {
                    CurrentRepository.Create(new T_PS_VOLUME_WORK
                    {
                        ID = headerId,
                        IS_CUSTOMER = model.IsCustomer,
                        PROJECT_ID = model.ProjectId,
                        TIME_PERIOD_ID = currentTimePeriod.ID,
                        STATUS = isSent == "0" ? ProjectWorkVolumeStatus.KHOI_TAO.GetValue() : ProjectWorkVolumeStatus.CHO_XAC_NHAN.GetValue(),
                        FROM_DATE = model.StartDate,
                        TO_DATE = model.FinishDate,
                        VENDOR_CODE = model.IsCustomer ? null : model.PartnerCode,
                        CREATE_BY = currentUser,
                        UPDATE_BY = currentUser,
                        UPDATE_TIMES = currentUpdateTimes + 1,
                    });
                    UnitOfWork.Repository<VolumeProgressHistoryRepo>()
                    .Create(new T_PS_VOLUME_PROGRESS_HISTORY
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = model.ProjectId,
                        ACTOR = currentUser,
                        CREATE_BY = currentUser,
                        ACTION = ProjectWorkVolumeAction.TAO_MOI.GetValue(),
                        DES_STATUS = ProjectWorkVolumeStatus.KHOI_TAO.GetValue(),
                        PRE_STATUS = ProjectWorkVolumeStatus.KHOI_TAO.GetValue(),
                        NOTE = yKien,
                        RESOURCE_ID = headerId,
                        IS_CUSTOMER = model.IsCustomer,
                        IS_ACCEPT = false
                    });
                    // Tạo email, notify khi nhấn Gửi luôn
                    if (isSent == "1")
                    {
                        List<string> lstEmail = new List<string>();
                        var lstProjectUser = UnitOfWork.Repository<ProjectResourceRepo>().Queryable().Where(x => x.PROJECT_ID == model.ProjectId).ToList();
                        var lstUserTNQS = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("TNQS")).ToList();
                        var lstUserSM = lstProjectUser.Where(x => !string.IsNullOrEmpty(x.PROJECT_ROLE_ID) && x.PROJECT_ROLE_ID.Contains("SM")).ToList();
                         

                        var loaiCV = string.Empty;
                        var urlDetail = string.Empty;

                        urlDetail = "/Home/OpenVolumeWork?id=" + headerId + "&partnerCode=" + model.PartnerCode + "&projectId=" + model.ProjectId + "&isCustomer=" + model.IsCustomer;
                        if (model.IsCustomer)
                        {
                            loaiCV = "Khối lượng thực hiện khách hàng";
                        }
                        else
                        {
                            loaiCV = "Khối lượng thực hiện thầu phụ";
                        }

                        string LOAI_CONG_VIEC = "{LOAI_CONG_VIEC}";
                        string MA = "{MA}";
                        string TEN = "{TEN}";
                        string Y_KIEN = "{Y_KIEN}";
                        string USER_NAME_NGUOI_DE_XUAT = "{USER_NAME_NGUOI_DE_XUAT}";
                        string HO_TEN_NGUOI_DE_XUAT = "{HO_TEN_NGUOI_DE_XUAT}";
                        string USER_NAME_NGUOI_PHE_DUYET = "{USER_NAME_NGUOI_PHE_DUYET}";
                        string HO_TEN_NGUOI_PHE_DUYET = "{HO_TEN_NGUOI_PHE_DUYET}";
                        string DON_VI_PHONG_BAN = "{DON_VI_PHONG_BAN}";
                        string TRANG_THAI = "{TRANG_THAI}";
                        string LINK_CHI_TIET = "{LINK_CHI_TIET}";
                        string VAI_TRO_TAI_DU_AN = "{VAI_TRO_TAI_DU_AN}";

                        var template = this.UnitOfWork.GetSession().Query<T_CF_TEMPLATE_NOTIFY>().FirstOrDefault();
                        var url = HttpContext.Current.Request.Url.Host;

                        var contentSubjectXuLy = template.CONG_VIEC_XU_LY_SUBJECT
                         .Replace(LOAI_CONG_VIEC, loaiCV)
                         .Replace(MA, project.CODE)
                         .Replace(TEN, project.NAME);


                        var contentBodyXuLy = template.CONG_VIEC_XU_LY_BODY
                                .Replace(LOAI_CONG_VIEC, loaiCV)
                                .Replace(MA, project.CODE)
                                .Replace(TEN, project.NAME)
                                .Replace(TRANG_THAI, "Chờ xác nhận")
                                .Replace(LINK_CHI_TIET, url + urlDetail)
                                .Replace(Y_KIEN, "Xác nhận dữ liệu thực hiện!")
                                .Replace(HO_TEN_NGUOI_DE_XUAT, ProfileUtilities.User.FULL_NAME);

                        foreach (var user in lstUserTNQS)
                        {
                            UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                EMAIL = user.User.EMAIL,
                                SUBJECT = contentSubjectXuLy,
                                CONTENTS = contentBodyXuLy.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                            });
                        }

                        foreach (var user in lstUserSM)
                        {
                            UnitOfWork.GetSession().Save(new T_CM_EMAIL()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                EMAIL = user.User.EMAIL,
                                SUBJECT = contentSubjectXuLy,
                                CONTENTS = contentBodyXuLy.Replace(VAI_TRO_TAI_DU_AN, user.PROJECT_ROLE_ID)
                            });
                        }


                        // Tạo notify
                        lstUserNotify.AddRange(lstUserTNQS.Select(x => x.USER_NAME).ToList());
                        foreach (var user in lstUserNotify)
                        {
                            var newId = Guid.NewGuid().ToString();

                            string strTemplate = @"
                            <a href='#' id='a{0}' onclick = 'SendNotifyIsReaded(""{0}""); Forms.LoadAjax(""{1}"");'>
                                <div class='icon-circle {2}'>
                                    <i class='material-icons'>{3}</i>
                                </div>
                                <div class='menu-info'>
                                    <span>Dự án [{4} - {5}] đang chờ xác nhận dữ liệu thực hiện!</span>
                                    <p>
                                        <i class='material-icons'>access_time</i> {6}
                                    </p>
                                </div>
                            </a>
                        ";

                            string strContent = string.Format(strTemplate,
                                newId,
                                $"/PS/Project/Edit?id={project.ID}",
                                "",
                                "",
                                project.CODE,
                                project.NAME,
                                DateTime.Now.ToString("dd/MM/yyyy HH:mm"));

                            string strRawContent = $"Dự án {project.CODE} đang chờ xác nhận dữ liệu thực hiện!";

                            this.UnitOfWork.GetSession().Save(new T_CM_NOTIFY()
                            {
                                PKID = newId,
                                CONTENTS = strContent,
                                RAW_CONTENTS = strRawContent,
                                USER_NAME = user
                            });
                        }
                    }
                }

                this.UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK_DETAIL>().Where(x => x.HEADER_ID == model.Id).Delete();

                UnitOfWork.Repository<VolumeWorkDetailRepo>().Create(lstObj:
                    (from detail in model.Details
                     select new T_PS_VOLUME_WORK_DETAIL
                     {
                         ID = Guid.NewGuid(),
                         HEADER_ID = headerId,
                         CONFIRM_VALUE = detail.AccumulateAllowedVolume,
                         REFERENCE_FILE_ID = Guid.NewGuid(),
                         PROJECT_STRUCT_ID = detail.ProjectStructureId,
                         CREATE_BY = currentUser,
                         UPDATE_BY = currentUser,
                         VALUE = detail.WorkVolume,
                         PRICE = detail.Price,
                         TOTAL = detail.WorkVolume * detail.Price,
                         NOTES = detail.Notes,
                     }).ToList());
                UnitOfWork.Commit();
                SMOUtilities.SendNotify(lstUserNotify);
                return headerId;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = e;
                return Guid.Empty;
            }
        }

        internal Guid RefreshData(AcceptTimesModel model)
        {
            if (!this.State)
            {
                return Guid.Empty;
            }

            UnitOfWork.Clear();
            var project = this.UnitOfWork.GetSession().Query<T_PS_PROJECT>().FirstOrDefault(x => x.ID == model.ProjectId);
            var allUpdateTimes = CurrentRepository.Queryable()
                .Where(x =>
                x.VENDOR_CODE == (model.IsCustomer ? null : model.PartnerCode)
                && x.PROJECT_ID == model.ProjectId
                && x.IS_CUSTOMER == model.IsCustomer)
                .ToList();
            var currentUpdateTimes = allUpdateTimes.Count == 0 ? 0 : allUpdateTimes.Max(x => x.UPDATE_TIMES);
            var timePeriods = GetProjectTime();
            var currentTimePeriod = timePeriods.FirstOrDefault(x => x.START_DATE <= model.FinishDate && x.FINISH_DATE >= model.FinishDate);

            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var headerId = Guid.NewGuid();
                UnitOfWork.BeginTransaction();
                if (model.Id != null && model.Id != Guid.Empty)
                {
                    headerId = model.Id;    
                }
                else
                {
                    CurrentRepository.Create(new T_PS_VOLUME_WORK
                    {
                        ID = headerId,
                        IS_CUSTOMER = model.IsCustomer,
                        PROJECT_ID = model.ProjectId,
                        TIME_PERIOD_ID = currentTimePeriod.ID,
                        FROM_DATE = model.StartDate,
                        TO_DATE = model.FinishDate,
                        VENDOR_CODE = model.IsCustomer ? null : model.PartnerCode,
                        UPDATE_TIMES = currentUpdateTimes,
                    });
                    UnitOfWork.Repository<VolumeProgressHistoryRepo>()
                    .Create(new T_PS_VOLUME_PROGRESS_HISTORY
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = model.ProjectId,
                        RESOURCE_ID = headerId,
                        IS_CUSTOMER = model.IsCustomer,
                        IS_ACCEPT = false
                    });
                }

                this.UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK_DETAIL>().Where(x => x.HEADER_ID == model.Id).Delete();

                UnitOfWork.Repository<VolumeWorkDetailRepo>().Create(lstObj:
                    (from detail in model.Details
                     select new T_PS_VOLUME_WORK_DETAIL
                     {
                         ID = Guid.NewGuid(),
                         HEADER_ID = headerId,
                         CONFIRM_VALUE = detail.AccumulateAllowedVolume,
                         REFERENCE_FILE_ID = Guid.NewGuid(),
                         PROJECT_STRUCT_ID = detail.ProjectStructureId,
                         VALUE = detail.WorkVolume,
                         PRICE = detail.Price,
                         TOTAL = detail.WorkVolume * detail.Price,
                         NOTES = detail.Notes,
                     }).ToList());
                UnitOfWork.Commit();
                return headerId;
            }
            catch (Exception e)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = e;
                return Guid.Empty;
            }
        }
        internal IList<RefreshTimePeriodWorkVolumeData> GetTimePeriodWorkVolumeData(RefreshTimePeriodModel model)
        {
            try
            {
                ObjDetail.PROJECT_ID = model.ProjectId;
                var projectTimes = GetProjectTime();
                var currentTimePeriods = projectTimes.Where(x =>
                {
                    var isStartDateInPeriod = model.StartDate >= x.START_DATE && model.StartDate <= x.FINISH_DATE;
                    var isEndDateInPeriod = model.FinishDate >= x.START_DATE && model.FinishDate <= x.FINISH_DATE;
                    var isPeriodInsideRange = model.FinishDate >= x.FINISH_DATE && model.StartDate <= x.START_DATE;

                    return isEndDateInPeriod || isStartDateInPeriod || isPeriodInsideRange;
                })
                    .Select(x => x.ID)
                    .ToList();
                var lastProjectTimeOrder = projectTimes.Where(x => x.START_DATE < model.FinishDate && x.FINISH_DATE >= model.StartDate)
                .Select(x => x.C_ORDER)
                .OrderByDescending(x => x)
                .FirstOrDefault();
                var previousProjectTimes = projectTimes.Where(projectTime => lastProjectTimeOrder >= projectTime.C_ORDER)
                    .Select(x => x.ID).ToList();
                var contracts = UnitOfWork.Repository<ContractRepo>().Queryable()
                    .Where(x => x.PROJECT_ID == model.ProjectId
                    && x.IS_SIGN_WITH_CUSTOMER == model.IsCustomer
                    && x.VENDOR_CODE == model.PartnerCode)
                    .Select(x => x.Details)
                    .ToList();
                if (currentTimePeriods.Count == 0)
                {
                    return (from contract in contracts
                            from contractDetail in contract
                            select new RefreshTimePeriodWorkVolumeData
                            {
                                ProjectStructureId = contractDetail.Struct.ID
                            }).ToList();
                }
                var allProjectWorkVolumesPheDuyet = CurrentRepository.Queryable()
                    .Where(x => x.TO_DATE <= model.FinishDate && x.PROJECT_ID == model.ProjectId && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                    .SelectMany(x => x.Details)
                    .ToList();
                var allProjectWorkVolumes = CurrentRepository.Queryable()
                    .Where(x => x.TO_DATE == model.FinishDate && x.PROJECT_ID == model.ProjectId && x.STATUS != ProjectWorkVolumeStatus.TU_CHOI.GetValue() && x.STATUS != ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue())
                    .SelectMany(x => x.Details)
                    .ToList();
                var allPlanVolumes = UnitOfWork.Repository<PlanCostRepo>().Queryable()
                    .Where(x => x.IS_CUSTOMER == model.IsCustomer
                    && x.PROJECT_ID == model.ProjectId
                    && previousProjectTimes.Contains(x.TIME_PERIOD_ID))
                    .ToList();

                
                var dataVolumeWork = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
                    .Where(x => x.ID == model.Id).FirstOrDefault();
                if (dataVolumeWork?.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue() || dataVolumeWork?.STATUS == ProjectWorkVolumeStatus.TU_CHOI.GetValue() || dataVolumeWork?.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue())
                {
                    return (from contract in contracts
                            from contractDetail in contract
                            let accumulatedPlanVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID).Sum(x => x.VALUE)
                            let planVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID
                            && currentTimePeriods.Contains(x.TIME_PERIOD_ID))
                            .Sum(x => x.VALUE)
                            let projectWorkVolumes = allProjectWorkVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            let projectWorkVolumesPheDuyet = allProjectWorkVolumesPheDuyet.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            select new RefreshTimePeriodWorkVolumeData
                            {
                                ProjectStructureId = contractDetail.PROJECT_STRUCT_ID,
                                PlanVolume = planVolume,
                                AccumulatedPlanVolume = accumulatedPlanVolume,
                                AccumulatedVolume = projectWorkVolumesPheDuyet.Sum(x => x.VALUE),
                                AccumulateAcceptedVolume = projectWorkVolumes.Sum(x => x.CONFIRM_VALUE),
                            })
                                            .ToList();
                }
                else
                {
                    return (from contract in contracts
                            from contractDetail in contract
                            let accumulatedPlanVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID).Sum(x => x.VALUE)
                            let planVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID
                            && currentTimePeriods.Contains(x.TIME_PERIOD_ID))
                            .Sum(x => x.VALUE)
                            let projectWorkVolumes = allProjectWorkVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            let currentValue = dataVolumeWork?.Details.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)

                            let projectWorkVolumesPheDuyet = allProjectWorkVolumesPheDuyet.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            select new RefreshTimePeriodWorkVolumeData
                            {
                                ProjectStructureId = contractDetail.PROJECT_STRUCT_ID,
                                PlanVolume = planVolume,
                                AccumulatedPlanVolume = accumulatedPlanVolume,
                                AccumulatedVolume = currentValue == null ? projectWorkVolumesPheDuyet.Sum(x => x.VALUE) : projectWorkVolumesPheDuyet.Sum(x => x.VALUE) + currentValue?.Sum(x => x.VALUE),
                                AccumulateAcceptedVolume = projectWorkVolumes.Sum(x => x.CONFIRM_VALUE),
                            })
                                            .ToList();
                }

            }
            catch
            {
                return new List<RefreshTimePeriodWorkVolumeData>();
            }

        }

        public override void Delete(string strLstSelected)
        {
            try
            {
                // Kiểm tra điều kiện xóa
                var lstStrID = strLstSelected.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var lstID = new List<Guid>();
                foreach (var item in lstStrID)
                {
                    var guid = new Guid();
                    if (Guid.TryParse(item, out guid))
                    {
                        lstID.Add(guid);
                    }
                }

                var find = this.UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK>().Where(x => lstID.Contains(x.ID)).ToList();
                if (find.Where(x => x.STATUS != ProjectWorkVolumeStatus.KHOI_TAO.GetValue() 
                && x.STATUS != ProjectWorkVolumeStatus.TU_CHOI.GetValue() && x.STATUS != ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue()).Count() > 0)
                {
                    this.State = false;
                    this.ErrorMessage = "Chỉ được phép xóa những đợt nhập dữ liệu ở trạng thái Khởi tạo hoặc Từ chối hoặc Không xác nhận!";
                    return;
                }

                UnitOfWork.BeginTransaction();
                UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK_DETAIL>().Where(x => lstID.Contains(x.HEADER_ID)).Delete();
                UnitOfWork.GetSession().Query<T_PS_VOLUME_WORK>().Where(x => lstID.Contains(x.ID)).Delete();
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                State = false;
                Exception = ex;
            }
        }
    }
}
