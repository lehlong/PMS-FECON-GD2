using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;
using SMO.Service.PS.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NPOI.HSSF.Util.HSSFColor;

namespace SMO.Service.PS
{
    public class DtSlModel
    {
        public Guid TimeId { get; set; }
        public decimal Value { get; set; }
        public decimal? Price { get; set; }
    }
    public class ProjectSlDtService : GenericService<T_PS_SL_DT, ProjectSlDtRepo>
    {
        public override void Search()
        {
            NumerRecordPerPage = int.MaxValue;
            var projectTimes = UnitOfWork.Repository<ProjectTimeRepo>().Queryable()
                        .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID).ToList();
            var criterias = UnitOfWork.Repository<CriteriaRepo>().GetListActive();
            base.Search();
            if (base.State && ObjList.Count == 0)
            {
                try
                {
                    UnitOfWork.BeginTransaction();
                    var currentUser = ProfileUtilities.User.USER_NAME;

                    var lst = (from time in projectTimes
                               from criteria in criterias
                               where criteria.CODE != ProjectCriteria.GIA_TRI_SAN_LUONG.GetValue() && criteria.CODE != ProjectCriteria.KE_HOACH_CHI_PHI.GetValue()
                               select new T_PS_SL_DT
                               {
                                   ID = Guid.NewGuid(),
                                   PROJECT_ID = ObjDetail.PROJECT_ID,
                                   TIME_PERIOD_ID = time.ID,
                                   CREATE_BY = currentUser,
                                   CRITERIA_CODE = criteria.CODE
                               }).ToList();
                    CurrentRepository.Create(lst);
                    UnitOfWork.Commit();
                    foreach (var item in lst)
                    {
                        CurrentRepository.Detach(item);
                    }
                    base.Search();
                }
                catch (Exception ex)
                {
                    UnitOfWork.Rollback();
                    this.State = false;
                    this.Exception = ex;
                }
            }


            var projectStructure = UnitOfWork.Repository<ProjectStructRepo>()
                .Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID)
                .ToList();
            ObjList.AddRange(GetCostValues(projectTimes,
                                           criterias,
                                           projectStructure.Where(x => x.TYPE == ProjectEnum.BOQ.ToString() || x.TYPE == ProjectEnum.PROJECT.ToString()).ToList(),
                                           isCustomer: true,
                                           code: ProjectCriteria.GIA_TRI_SAN_LUONG.GetValue()));
            ObjList.AddRange(GetCostValues(projectTimes,
                                           criterias,
                                           projectStructure.Where(x => x.TYPE != ProjectEnum.BOQ.ToString()).ToList(),
                                           isCustomer: false,
                                           code: ProjectCriteria.KE_HOACH_CHI_PHI.GetValue()));
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
            styleCellNumber.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("#,##0.000");
            return styleCellNumber;
        }
        private ICellStyle GetCellStylePercentage(IWorkbook templateWorkbook)
        {
            ICellStyle styleCellPercentage = templateWorkbook.CreateCellStyle();
            styleCellPercentage.DataFormat = templateWorkbook.CreateDataFormat().GetFormat("0.000%");
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
                            var isHeaderVolume = header.Any(x => x.ToList()[j].Contains("KL") || x.ToList()[j].ToLower().Contains("khối lượng"));
                            rowCur.Cells[j].CellStyle = config.ColumnsPercentage.Contains(j)
                                || (isRowPercentageUnit && config.ColumnsVolume.Contains(j)) ?
                                styleCellNumberDecimal :
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


        internal IEnumerable<T_PS_TIME> GetProjectTime()
        {
            return UnitOfWork.Repository<ProjectTimeRepo>().Queryable()
                        .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID)
                        .OrderBy(x => x.C_ORDER)
                        .ToList();
        }

        internal DateTime? GetNgayQuyetToan(Guid projectId)
        {
            return UnitOfWork.Repository<ProjectRepo>().Queryable().FirstOrDefault(x => x.ID == projectId)?.NGAY_QUYET_TOAN;
        }

        internal DateTime? GetHanBaoHanh(Guid projectId)
        {
            return UnitOfWork.Repository<ProjectRepo>().Queryable().FirstOrDefault(x => x.ID == projectId)?.HAN_BAO_HANH;
        }

        private IEnumerable<T_PS_SL_DT> GetCostValues(IList<T_PS_TIME> projectTimes,
                                                      IList<T_MD_CRITERIA> criterias,
                                                      IList<T_PS_PROJECT_STRUCT> projectStructure,
                                                      bool isCustomer,
                                                      string code)
        {
            var costValues = UnitOfWork.Repository<PlanCostRepo>()
                .Queryable()
                .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID && x.IS_CUSTOMER == isCustomer)
                .ToList();
            var criteria = criterias.FirstOrDefault(x => x.CODE == code);

            var costSum = (from x in costValues
                           join y in projectStructure on x.PROJECT_STRUCT_ID equals y.ID
                           select new
                           {
                               Id = y.ID,
                               Parent = y.PARENT_ID,
                               TimeId = x.TIME_PERIOD_ID,
                               Value = x.VALUE,
                               Price = y.PRICE
                           }).ToList();
            var data = from x in costSum
                       let checkParent = costSum.Any(y => y.Id == x.Parent && x.TimeId == y.TimeId && y.Value != 0)
                       select new
                       {
                           TimeId = x.TimeId,
                           Price = checkParent || x.Price == null ? 0 : x.Price,
                           Value = checkParent || x.Value == null ? 0 : x.Value,
                       };

            var valueCost = from x in data
                            group x by x.TimeId into y
                            select new
                            {
                                timeId = y.Key,
                                value = y.Sum(a => a.Price * a.Value)
                            };


            //var valueCost = from x in costValues
            //              group x by x.TIME_PERIOD_ID into y
            //              select new
            //              {
            //                  timeId = y.Key,
            //                  value = y.Sum(a => a.VALUE)
            //              };


            return from projectTime in projectTimes
                   join cost in valueCost on projectTime.ID equals cost.timeId
                   select new T_PS_SL_DT
                   {
                       ID = Guid.NewGuid(),
                       PROJECT_ID = ObjDetail.PROJECT_ID,
                       CRITERIA_CODE = code,
                       TIME_PERIOD_ID = projectTime.ID,
                       VALUE = cost.value ?? 0,
                       TimePeriod = projectTime,
                       Criteria = criteria
                   };
        }

        private decimal CalculateTotalDescendantPlanCost(IEnumerable<T_PS_PROJECT_STRUCT> projectStructure, IDictionary<Guid, decimal> dictValue)
        {
            var dumpDictValue = new Dictionary<Guid, decimal>();
            foreach (var structure in projectStructure)
            {
                if (!dictValue.ContainsKey(structure.ID))
                {
                    dumpDictValue.Add(structure.ID, 0);
                }
                else
                {
                    dumpDictValue.Add(structure.ID, dictValue[structure.ID]);
                }
            }
            var structureTotalDic = new Dictionary<Guid, decimal>();
            foreach (var structure in projectStructure.OrderByDescending(x => x.C_ORDER))
            {
                if (dumpDictValue[structure.ID] > 0 && structure.PRICE > 0)
                {
                    continue;
                }
                var children = projectStructure.Where(x => x.PARENT_ID == structure.ID).ToList();
                decimal _dumpTotal = 0;
                decimal? total = (from child in children
                                  let hasStructureTotal = structureTotalDic.TryGetValue(child.ID, out _dumpTotal)
                                  let _dumpPlanCostValue = dumpDictValue[child.ID]
                                  select hasStructureTotal ? _dumpTotal : child.PRICE * _dumpPlanCostValue).Sum();
                structureTotalDic.Add(structure.ID, total ?? 0);

            }
            var projectStructureType = projectStructure.FirstOrDefault(x => x.TYPE == ProjectEnum.PROJECT.ToString());
            decimal result = 0;
            var hasProjectValue = structureTotalDic.TryGetValue(projectStructureType.ID, out result);
            return result;
        }

        public decimal CalculateTotal(IEnumerable<T_PS_PROJECT_STRUCT> projectStructure, IDictionary<Guid, decimal> dictValue)
        {
            return (from value in dictValue
                    let price = projectStructure.FirstOrDefault(x => x.ID == value.Key)?.PRICE ?? 0
                    select value.Value * price).Sum();
        }

        internal decimal CalculateContractTotal(IEnumerable<T_PS_CONTRACT_DETAIL> contractDetails, Dictionary<Guid, decimal> dictionary)
        {
            return (from value in dictionary
                    let price = contractDetails.FirstOrDefault(x => x.PROJECT_STRUCT_ID == value.Key)?.UNIT_PRICE ?? 0
                    select value.Value * price).Sum();
        }
        internal void UpdateValue(UpdateValueModel model)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var currentObj = CurrentRepository.Queryable()
                    .Where(x => x.PROJECT_ID == model.ProjectId
                    && x.TIME_PERIOD_ID == model.PeriodId
                    && x.CRITERIA_CODE == model.CriteriaCode)
                    .FirstOrDefault();
                var currentUser = ProfileUtilities.User?.USER_NAME;
                if (currentObj == null)
                {
                    CurrentRepository.Create(new T_PS_SL_DT
                    {
                        ID = Guid.NewGuid(),
                        CREATE_BY = currentUser,
                        CRITERIA_CODE = model.CriteriaCode,
                        PROJECT_ID = model.ProjectId,
                        TIME_PERIOD_ID = model.PeriodId,
                        VALUE = model.Value
                    });
                }
                else
                {
                    CurrentRepository.Detach(currentObj);

                    currentObj.VALUE = model.Value;
                    currentObj.UPDATE_BY = currentUser;
                    CurrentRepository.Update(currentObj);
                }

                UnitOfWork.Repository<ProjectRepo>().ResetStatus(model.ProjectId, currentUser, "Kế hoạch doanh thu dòng tiền");
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
        protected T_PS_PROJECT GetProject(Guid projectId)
        {
            return UnitOfWork.Repository<ProjectRepo>().Get(projectId);
        }
        protected string GetProjectTimeTypeString(Guid projectId)
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

        internal IList<T_PS_TIME> GetListProjectTime()
        {
            return UnitOfWork.Repository<ProjectTimeRepo>().Queryable()
                        .Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID)
                        .OrderBy(x => x.C_ORDER)
                        .ToList();
        }

        internal void SetHeaderExportExcel(ref MemoryStream outFileHeaderStream, MemoryStream outFileStream, Guid projectId, int startCol)
        {
            ObjDetail.PROJECT_ID = projectId;
            var projectTimes = GetListProjectTime();
            const int COL_PER_PERIOD = 2;
            try
            {
                outFileStream.Position = 0;
                IWorkbook templateWorkbook = new XSSFWorkbook(outFileStream);
                outFileStream.Close();
                ISheet sheet = templateWorkbook.GetSheetAt(0);
                var timeType = GetProjectTimeTypeString(projectId);
                var rowHeaderPeriod = sheet.GetRow(6);
                var rowHeaderValue = sheet.GetRow(7);
                int numberPeriods = projectTimes.Count();

                ReportUtilities.CreateCell(ref rowHeaderPeriod, numberPeriods * COL_PER_PERIOD + startCol + 1);
                ReportUtilities.CreateCell(ref rowHeaderValue, numberPeriods * COL_PER_PERIOD + startCol + 1);
                for (int index = 0; index < numberPeriods; index++)
                {
                    var timePeriod = projectTimes[index];
                    if (index == 0)
                    {
                        rowHeaderPeriod.Cells[startCol - COL_PER_PERIOD + 1].SetCellValue($"{timeType.ToUpper()} {index + 1} ({timePeriod.START_DATE.ToString(Global.DateToStringFormat)}-{timePeriod.FINISH_DATE.ToString(Global.DateToStringFormat)})");
                    }
                    else
                    {
                        int i = index - 1;
                        var cra = new CellRangeAddress(firstRow: 6, lastRow: 6, firstCol: i * COL_PER_PERIOD + startCol + 1, lastCol: i * COL_PER_PERIOD + startCol + COL_PER_PERIOD);
                        sheet.AddMergedRegion(cra);

                        if (index == numberPeriods - 1 || index == numberPeriods - 2)
                        {
                            rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 1, i * COL_PER_PERIOD + startCol + 1);
                            rowHeaderValue.Cells[startCol - COL_PER_PERIOD + 1].CopyCellTo(i * COL_PER_PERIOD + startCol + 1).SetCellValue("Tiền trong kỳ");
                            sheet.SetColumnWidth(i * COL_PER_PERIOD + startCol + 1, sheet.GetColumnWidth(startCol - COL_PER_PERIOD + 1));

                            rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 2, i * COL_PER_PERIOD + startCol + 2);
                            rowHeaderValue.Cells[startCol - COL_PER_PERIOD + 2].CopyCellTo(i * COL_PER_PERIOD + startCol + 2).SetCellValue("Tiền luỹ kế");
                            sheet.SetColumnWidth(i * COL_PER_PERIOD + startCol + 2, sheet.GetColumnWidth(startCol - COL_PER_PERIOD + 2));

                            var firstDayOfMonth = new DateTime(timePeriod.START_DATE.Year, timePeriod.START_DATE.Month, 1);
                            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);
                            if (index == numberPeriods - 1)
                            {
                                rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 1, i * COL_PER_PERIOD + startCol + 1).SetCellValue($"KỲ HẾT BẢO HÀNH ({firstDayOfMonth.ToString(Global.DateToStringFormat)}-{lastDayOfMonth.ToString(Global.DateToStringFormat)})");
                            }
                            else if(index == numberPeriods - 2)
                            {
                                rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 1, i * COL_PER_PERIOD + startCol + 1).SetCellValue($"KỲ QUYẾT TOÁN ({firstDayOfMonth.ToString(Global.DateToStringFormat)}-{lastDayOfMonth.ToString(Global.DateToStringFormat)})");
                            }
                            
                        }
                        else
                        {
                            rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 1, i * COL_PER_PERIOD + startCol + 1);
                            rowHeaderValue.Cells[startCol - COL_PER_PERIOD + 1].CopyCellTo(i * COL_PER_PERIOD + startCol + 1).SetCellValue($"Tháng {index + 1}");
                            sheet.SetColumnWidth(i * COL_PER_PERIOD + startCol + 1, sheet.GetColumnWidth(startCol - COL_PER_PERIOD + 1));

                            rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 2, i * COL_PER_PERIOD + startCol + 2);
                            rowHeaderValue.Cells[startCol - COL_PER_PERIOD + 2].CopyCellTo(i * COL_PER_PERIOD + startCol + 2).SetCellValue($"LK Tháng {index + 1}");
                            sheet.SetColumnWidth(i * COL_PER_PERIOD + startCol + 2, sheet.GetColumnWidth(startCol - COL_PER_PERIOD + 2));

                            rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 1, i * COL_PER_PERIOD + startCol + 1).SetCellValue($"{timeType.ToUpper()} {index + 1} ({timePeriod.START_DATE.ToString(Global.DateToStringFormat)}-{timePeriod.FINISH_DATE.ToString(Global.DateToStringFormat)})");
                        }                       
                    }

                }
                templateWorkbook.Write(outFileHeaderStream);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.ErrorMessage = "Có lỗi xảy ra trong quá trình tạo file excel!";
                this.Exception = ex;
            }
        }

    }
}
