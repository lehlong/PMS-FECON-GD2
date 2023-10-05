using NHibernate.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using SMO.Service.Common;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SMO.Service.PS
{
    public class VolumeAcceptService : BaseProjectVolumeService<T_PS_VOLUME_ACCEPT, VolumeAcceptRepo>
    {
        public IList<AcceptVolumeViewModel> GetAcceptVolumes()
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

            var allProjectWorkVolumes = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
                .Where(x => x.TO_DATE <= ObjDetail.TO_DATE && x.PROJECT_ID == ObjDetail.PROJECT_ID)
                .SelectMany(x => x.Details)
                .ToList();
            var lstProjectAcceptVolumes = CurrentRepository.Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID && x.STATUS != ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue() && x.STATUS != ProjectWorkVolumeStatus.TU_CHOI.GetValue())
                .SelectMany(x => x.Details)
                .ToList();
            var allProjectAcceptVolumes = CurrentRepository.Queryable()
                .Where(x => x.TO_DATE == ObjDetail.TO_DATE && x.PROJECT_ID == ObjDetail.PROJECT_ID && x.ID == ObjDetail.ID)
                .SelectMany(x => x.Details)
                .ToList();
            var allProjectAcceptVolumesPheDuyet = CurrentRepository.Queryable()
                .Where(x => x.TO_DATE <= ObjDetail.TO_DATE && x.PROJECT_ID == ObjDetail.PROJECT_ID && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
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
                    let projectAcceptVolumes = allProjectAcceptVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let projectAcceptVolumesPheDuyet = allProjectAcceptVolumesPheDuyet.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let acceptVolumes = lstProjectAcceptVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let projectWorkVolumes = allProjectWorkVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                    let accumulatedPlanVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID).Sum(x => x.VALUE)
                    let planVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID
                    && inProjectTimes.Contains(x.TIME_PERIOD_ID))
                    .Sum(x => x.VALUE)
                    let volumeAcceptDetail = ObjDetail.Details.FirstOrDefault(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)

                    let unitPriceContract = volumeAcceptDetail?.PRICE ?? (ObjDetail.IS_CUSTOMER ? projectStructures.FirstOrDefault(x => x.ID == contractDetail.PROJECT_STRUCT_ID)?.PRICE : contractDetail.UNIT_PRICE)
                    select ConvertToViewModel(acceptVolumes, projectAcceptVolumes, projectAcceptVolumesPheDuyet, projectWorkVolumes, contractDetail, contract, planVolume, accumulatedPlanVolume, unitPriceContract))
                    .OrderBy(x => x.Order)
                    .ToList();
        }

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
            styleCellPercentage.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("0.00%");
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
                            rowCur.Cells[j].CellStyle = config.ColumnsPercentage.Contains(j) || (isRowPercentageUnit && config.ColumnsVolume.Contains(j)) ?styleCellPercentage : isHeaderVolume ? styleCellNumberDecimal : styleCellNumber;
                            rowCur.Cells[j].SetCellValue(canParseNumber ? value:0);
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

        internal AcceptVolumeViewModel ConvertToViewModel(
            IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> acceptVolumes,
            IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> projectAcceptVolumes,
            IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> projectAcceptVolumesPheDuyet,
            IEnumerable<T_PS_VOLUME_WORK_DETAIL> projectWorkVolumes,
            T_PS_CONTRACT_DETAIL contractDetail,
            T_PS_CONTRACT contract,
            decimal planVolume,
            decimal accumulatedPlanVolume,
            decimal? unitPriceContract)
        {
            var currentValue = ObjDetail.Details.FirstOrDefault(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID);
            //if (ObjDetail.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())

            return new AcceptVolumeViewModel
            {
                Id = currentValue?.ID,
                ProjectStructureId = contractDetail.PROJECT_STRUCT_ID,
                ParentId = contractDetail.Struct.PARENT_ID,
                Notes = currentValue?.NOTES,
                ProjectId = ObjDetail.PROJECT_ID,
                AcceptVolume = currentValue?.VALUE,
                Price = currentValue?.PRICE ?? unitPriceContract,
                ReferenceFileId = currentValue?.REFERENCE_FILE_ID,
                ContractName = contract.NAME,
                ContractCode = contract.CONTRACT_NUMBER,
                ContractDetailId = contractDetail.ID,
                ContractVolume = ObjDetail.IS_CUSTOMER ? contractDetail.Struct.QUANTITY : contractDetail.VOLUME,
                ProjectStructureType = contractDetail.Struct.TYPE,
                ProjectStructureCode = contractDetail.Struct.GEN_CODE,
                ProjectStructureName = contractDetail.Struct.TEXT,
                UnitCode = contractDetail.Struct.UNIT_CODE,
                PlanVolume = planVolume,
                AccumulatedPlanVolume = accumulatedPlanVolume,
                Order = contractDetail.Struct.C_ORDER,
                AccumulatedVolume = projectAcceptVolumesPheDuyet.Sum(x => x.VALUE),
                AccumulateAcceptedVolume = projectWorkVolumes.Sum(x => x.CONFIRM_VALUE),
                AccumulateAllowedVolume = (ObjDetail.STATUS == ProjectWorkVolumeStatus.TU_CHOI.GetValue() || ObjDetail.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue()) ? projectAcceptVolumesPheDuyet.Sum(x => x.VALUE) : projectAcceptVolumesPheDuyet.Sum(x => x.VALUE) + currentValue?.VALUE,
                TotalAccumulateAllowedVolume = acceptVolumes.Sum(x => x.VALUE),
                WorkStatus = contractDetail.Struct.STATUS.GetEnum<ProjectStructStatus>().GetName(),
                UserUpdated = currentValue?.UPDATE_BY
            };
        }
        public override ProjectWorkVolumeStatus UpdateStatus(UpdateStatusVolumeModel model, bool isAccept)
        {
            var statusFromAction = GetStatusFromAction(model.Action.GetEnum<ProjectWorkVolumeAction>());
            if (statusFromAction.GetValue() == ProjectWorkVolumeStatus.XAC_NHAN.GetValue() ||
                statusFromAction.GetValue() == ProjectWorkVolumeStatus.PHE_DUYET.GetValue()
                )
            {
                var projectStructureIds = UnitOfWork.Repository<VolumeWorkDetailRepo>().Queryable()
                    .Where(x => x.HEADER_ID == model.Id).Select(x => x.PROJECT_STRUCT_ID)
                    .ToList();
                var currentObj = CurrentRepository.Get(model.Id);
                CurrentRepository.Detach(currentObj);
                ValidateDupplicate(projectStructureIds, currentObj.FROM_DATE, currentObj.TO_DATE, currentObj.PROJECT_ID, currentObj.ID,
                    currentObj.IS_CUSTOMER, currentObj.VENDOR_CODE);
                if (!this.State)
                {
                    return ProjectWorkVolumeStatus.CHO_XAC_NHAN;
                }
            }

            if (this.State)
            {
                return base.UpdateStatus(model, isAccept);
            }
            return ProjectWorkVolumeStatus.CHO_XAC_NHAN;
        }

        internal IEnumerable<T_PS_VOLUME_ACCEPT_DETAIL> GetAllVolumeAcceptDetail()
        {
            return UnitOfWork.Repository<VolumeAcceptDetailRepo>().GetAll().ToList();
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

            model.Details = model.Details.Where(x => x.AcceptVolume != 0).ToList();
            if (model.Details.Count() == 0)
            {
                this.State = false;
                this.ErrorMessage = "Hãy nhập ít nhất một hạng mục có giá trị khác 0!";
                return;
            }
            var projectStructureIds = model.Details.Select(x => x.ProjectStructureId).ToList();
            ValidateDupplicate(projectStructureIds, model.StartDate, model.FinishDate, model.ProjectId, model.Id, model.IsCustomer, model.PartnerCode);
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
            else
            {
                queryHeader = queryHeader.Where(x => x.IS_CUSTOMER == true);
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
                var structureIdExistInPeriod = UnitOfWork.Repository<VolumeAcceptDetailRepo>().Queryable()
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
        internal Guid AddUpdateTimes(AcceptTimesModel model , string isSent, string yKien)
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
            ObjDetail.PROJECT_ID = model.ProjectId;
            var timePeriods = GetProjectTime();
            var currentTimePeriod = timePeriods.FirstOrDefault(x => x.START_DATE <= model.FinishDate && x.FINISH_DATE >= model.FinishDate);

            if (currentTimePeriod == null)
            {
                State = false;
                ErrorMessage = "Thời gian nằm ngoài khoảng thời gian của dự án.";
                return Guid.Empty;
            }

            try
            {
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var headerId = Guid.NewGuid();
                if (model.Id != null && model.Id != Guid.Empty)
                {
                    headerId = model.Id;
                    if (isSent == "1")
                    {
                        UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT>().Where(x => x.ID == model.Id)
                        .Update(x => new T_PS_VOLUME_ACCEPT()
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
                        UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT>().Where(x => x.ID == model.Id)
                        .Update(x => new T_PS_VOLUME_ACCEPT()
                        {
                            FROM_DATE = model.StartDate,
                            TO_DATE = model.FinishDate,
                            UPDATE_BY = currentUser
                        });
                    }
                }
                else
                {
                    CurrentRepository.Create(new T_PS_VOLUME_ACCEPT
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
                        IS_ACCEPT = true
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

                        urlDetail = "/Home/OpenVolumeAccept?id=" + headerId + "&partnerCode=" + model.PartnerCode + "&projectId=" + model.ProjectId + "&isCustomer=" + model.IsCustomer;
                        if (model.IsCustomer)
                        {
                            loaiCV = "Khối lượng nghiệm thu khách hàng";
                        }
                        else
                        {
                            loaiCV = "Khối lượng nghiệm thu thầu phụ";
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
                                .Replace(Y_KIEN, "Xác nhận dữ liệu nghiệm thu!")
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
                                    <span>Dự án [{4} - {5}] đang chờ xác nhận dữ liệu nghiệm thu!</span>
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

                            string strRawContent = $"Dự án {project.CODE} đang chờ xác nhận dữ liệu nghiệm thu!";

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
                UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT_DETAIL>().Where(x => x.HEADER_ID == model.Id).Delete();
                UnitOfWork.Repository<VolumeAcceptDetailRepo>().Create(lstObj:
                    (from detail in model.Details
                     select new T_PS_VOLUME_ACCEPT_DETAIL
                     {
                         ID = Guid.NewGuid(),
                         HEADER_ID = headerId,
                         CONFIRM_VALUE = detail.AccumulateAllowedVolume,
                         REFERENCE_FILE_ID = Guid.NewGuid(),
                         PROJECT_STRUCT_ID = detail.ProjectStructureId,
                         CREATE_BY = currentUser,
                         UPDATE_BY = currentUser,
                         VALUE = detail.AcceptVolume,
                         PRICE = detail.Price,
                         TOTAL = detail.AcceptVolume * detail.Price,
                         NOTES = detail.Notes
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
            ObjDetail.PROJECT_ID = model.ProjectId;
            var timePeriods = GetProjectTime();
            var currentTimePeriod = timePeriods.FirstOrDefault(x => x.START_DATE <= model.FinishDate && x.FINISH_DATE >= model.FinishDate);

            try
            {
                UnitOfWork.BeginTransaction();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                var headerId = Guid.NewGuid();
                if (model.Id != null && model.Id != Guid.Empty)
                {
                    headerId = model.Id;
                }
                else
                {
                    CurrentRepository.Create(new T_PS_VOLUME_ACCEPT
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
                        IS_ACCEPT = true
                    });
                }
                UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT_DETAIL>().Where(x => x.HEADER_ID == model.Id).Delete();
                UnitOfWork.Repository<VolumeAcceptDetailRepo>().Create(lstObj:
                    (from detail in model.Details
                     select new T_PS_VOLUME_ACCEPT_DETAIL
                     {
                         ID = Guid.NewGuid(),
                         HEADER_ID = headerId,
                         CONFIRM_VALUE = detail.AccumulateAllowedVolume,
                         REFERENCE_FILE_ID = Guid.NewGuid(),
                         PROJECT_STRUCT_ID = detail.ProjectStructureId,
                         VALUE = detail.AcceptVolume,
                         PRICE = detail.Price,
                         TOTAL = detail.AcceptVolume * detail.Price,
                         NOTES = detail.Notes
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

        internal IList<RefreshTimePeriodAcceptVolumeData> GetTimePeriodProjectVolumeData(RefreshTimePeriodModel model)
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
                    .Select(x => x.ID);

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
                            select new RefreshTimePeriodAcceptVolumeData
                            {
                                ProjectStructureId = contractDetail.Struct.ID
                            }).ToList();
                }
                var allProjectAcceptVolumes = CurrentRepository.Queryable()
                    .Where(x => x.TO_DATE == model.FinishDate && x.FROM_DATE == model.StartDate && x.PROJECT_ID == model.ProjectId && x.STATUS != ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue() && x.STATUS != ProjectWorkVolumeStatus.TU_CHOI.GetValue())
                    .SelectMany(x => x.Details)
                    .ToList();
                var allProjectAcceptVolumesPheDuyet = CurrentRepository.Queryable()
                    .Where(x => x.TO_DATE <= model.FinishDate && x.PROJECT_ID == model.ProjectId && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                    .SelectMany(x => x.Details)
                    .ToList();
                var allPlanVolumes = UnitOfWork.Repository<PlanCostRepo>().Queryable()
                    .Where(x => x.IS_CUSTOMER == model.IsCustomer
                    && x.PROJECT_ID == model.ProjectId
                    && previousProjectTimes.Contains(x.TIME_PERIOD_ID))
                    .ToList();

                var allProjectWorkVolumes = UnitOfWork.Repository<VolumeWorkRepo>().Queryable()
                    .Where(x => x.TO_DATE <= model.FinishDate && x.PROJECT_ID == model.ProjectId && x.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue())
                    .SelectMany(x => x.Details)
                    .ToList();

                var dataVolumeAccept = UnitOfWork.Repository<VolumeAcceptRepo>().Queryable()
                    .Where(x => x.ID == model.Id).FirstOrDefault();
                if (dataVolumeAccept?.STATUS == ProjectWorkVolumeStatus.PHE_DUYET.GetValue() || dataVolumeAccept?.STATUS == ProjectWorkVolumeStatus.TU_CHOI.GetValue() || dataVolumeAccept?.STATUS == ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue())
                {
                    return (from contract in contracts
                            from contractDetail in contract
                            let accumulatedPlanVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID).Sum(x => x.VALUE)
                            let planVolume = allPlanVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID
                            && currentTimePeriods.Contains(x.TIME_PERIOD_ID))
                            .Sum(x => x.VALUE)
                            let projectWorkVolumes = allProjectWorkVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            let projectAcceptVolumes = allProjectAcceptVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            let projectAcceptVolumesPheDuyet = allProjectAcceptVolumesPheDuyet.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            select new RefreshTimePeriodAcceptVolumeData
                            {
                                ProjectStructureId = contractDetail.PROJECT_STRUCT_ID,
                                PlanVolume = planVolume,
                                AccumulatedPlanVolume = accumulatedPlanVolume,
                                AccumulatedVolume = projectWorkVolumes.Sum(x => x.VALUE),
                                AccumulateAcceptedVolume = projectWorkVolumes.Sum(x => x.CONFIRM_VALUE),
                                AccumulateAllowedVolume = projectAcceptVolumesPheDuyet.Sum(x => x.VALUE),
                            }).ToList();
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
                            let currentValue = dataVolumeAccept?.Details.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)

                            let projectAcceptVolumes = allProjectAcceptVolumes.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            let projectAcceptVolumesPheDuyet = allProjectAcceptVolumesPheDuyet.Where(x => x.PROJECT_STRUCT_ID == contractDetail.PROJECT_STRUCT_ID)
                            select new RefreshTimePeriodAcceptVolumeData
                            {
                                ProjectStructureId = contractDetail.PROJECT_STRUCT_ID,
                                PlanVolume = planVolume,
                                AccumulatedPlanVolume = accumulatedPlanVolume,
                                AccumulatedVolume = projectWorkVolumes.Sum(x => x.VALUE),
                                AccumulateAcceptedVolume = projectWorkVolumes.Sum(x => x.CONFIRM_VALUE),
                                AccumulateAllowedVolume = currentValue == null ? projectAcceptVolumesPheDuyet.Sum(x => x.VALUE) : projectAcceptVolumesPheDuyet.Sum(x => x.VALUE) + currentValue?.Sum(x => x.VALUE),
                            }).ToList();
                }
            }
            catch
            {
                State = false;
                return new List<RefreshTimePeriodAcceptVolumeData>();
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

                var find = this.UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT>().Where(x => lstID.Contains(x.ID)).ToList();
                if (find.Where(x => x.STATUS != ProjectWorkVolumeStatus.KHOI_TAO.GetValue() && x.STATUS != ProjectWorkVolumeStatus.TU_CHOI.GetValue() 
                && x.STATUS != ProjectWorkVolumeStatus.KHONG_XAC_NHAN.GetValue()).Count() > 0)
                {
                    this.State = false;
                    this.ErrorMessage = "Chỉ được phép xóa những đợt nhập dữ liệu ở trạng thái Khởi tạo hoặc Từ chối hoặc Không xác nhận!";
                    return;
                }

                UnitOfWork.BeginTransaction();
                UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT_DETAIL>().Where(x => lstID.Contains(x.HEADER_ID)).Delete();
                UnitOfWork.GetSession().Query<T_PS_VOLUME_ACCEPT>().Where(x => lstID.Contains(x.ID)).Delete();
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
