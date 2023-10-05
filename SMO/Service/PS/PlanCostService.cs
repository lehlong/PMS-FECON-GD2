using NHibernate.Linq;

using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;

using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;
using SMO.Repository.Implement.PS;
using SMO.Service.Common;
using SMO.Service.PS.Models;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SMO.Service.PS
{
    public class PlanCostService : BaseProjectPlanService<T_PS_PLAN_COST, PlanCostRepo>
    {
        public IList<PlanCostViewModel> GetPlanCosts()
        {
            NumerRecordPerPage = int.MaxValue;
            var contractIds = UnitOfWork.Repository<ContractRepo>().Queryable().Where(x => x.PROJECT_ID == ObjDetail.PROJECT_ID)
                .Select(x => x.ID)
                .ToList();
            var queryContractDetail = UnitOfWork.Repository<ContractDetailRepo>().Queryable()
                                    .Where(x => contractIds.Contains(x.CONTRACT_ID));

            var hasFilerVendor = !string.IsNullOrWhiteSpace(Vendor);
            if (hasFilerVendor)
            {
                queryContractDetail = queryContractDetail.Where(x => x.Contract.VENDOR_CODE.Equals(Vendor));
            }
            var contractDetails = queryContractDetail.ToList();
            var projectStructs = GetProjectStruct(contractDetails);
            var projectTimes = GetProjectTime();
            var planProgresses = GetPlanDesigns();
            var lstUnit = UnitOfWork.GetSession().Query<T_MD_UNIT>().ToList();
            base.Search();

            var project = GetProject(ObjDetail.PROJECT_ID);
            var projectTotal = ObjDetail.IS_CUSTOMER ? project?.TOTAL_BOQ : project?.TOTAL_COST;
            return (from projectStruct in projectStructs.OrderBy(x => x.C_ORDER)
                    from time in projectTimes
                    let currentValue = ObjList.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID && x.TIME_PERIOD_ID == time.ID)
                    let planProgress = planProgresses.FirstOrDefault(x => x.PROJECT_STRUCT_ID == projectStruct.ID)
                    select new PlanCostViewModel
                    {
                        Id = projectStruct.ID,
                        ProjectId = ObjDetail.PROJECT_ID,
                        Order = projectStruct.C_ORDER,
                        TimePeriodId = time.ID,
                        TimePeriodOrder = time.C_ORDER,
                        ParentId = projectStruct.PARENT_ID,
                        GenCode = projectStruct.GEN_CODE,
                        ProjectStructureId = projectStruct.ID,
                        ProjectStructureName = projectStruct.TEXT,
                        Quantity = projectStruct?.QUANTITY,
                        Price = projectStruct?.PRICE,
                        PlanVolume = projectStruct?.PLAN_VOLUME,
                        ThanhTien = projectStruct.TYPE == ProjectEnum.PROJECT.ToString() ? projectTotal : projectStruct?.TOTAL,
                        ProjectStructureType = projectStruct.TYPE,
                        UnitCode = projectStruct.UNIT_CODE,
                        UnitName = lstUnit.FirstOrDefault(x => x.CODE == projectStruct.UNIT_CODE)?.NAME,
                        StartDate = projectStruct.START_DATE,
                        FinishDate = projectStruct.FINISH_DATE,
                        Duration = (int)((projectStruct.FINISH_DATE - projectStruct.START_DATE).TotalDays) + 1,
                        Value = currentValue?.VALUE ?? 0,
                        PeriodTotal = currentValue?.TOTAL > 0 ? currentValue?.TOTAL : currentValue?.VALUE * projectStruct?.PRICE ?? 0
                    }).ToList();
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
                            if (!config.ColumnsNormal.Contains(j))
                            {
                                double value = 0;
                                var canParseNumber = double.TryParse(valueStr, out value);
                                var isHeaderVolume = header.Any(x => x.ToList()[j].Contains("KL") || x.ToList()[j].ToLower().Contains("khối lượng"));
                                rowCur.Cells[j].CellStyle = config.ColumnsPercentage.Contains(j)
                                    || (isRowPercentageUnit && config.ColumnsVolume.Contains(j)) ?
                                    styleCellNumberDecimal :
                                    isHeaderVolume ? styleCellNumberDecimal : styleCellNumber;
                                rowCur.Cells[j].SetCellValue(canParseNumber ? value : 0);
                                if (dataRow.ElementAt(3) == "%" && isHeaderVolume)
                                {
                                    rowCur.Cells[j].CellStyle = styleCellPercentage;
                                    rowCur.Cells[j].SetCellValue(value);
                                }
                            }
                            else
                            {
                                rowCur.Cells[j].SetCellValue(valueStr);

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

        internal void UpdateParentTotal(UpdatePlanCostValueModel model)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.BeginTransaction();
                // update khoi luong trong ky
                var currentObj = CurrentRepository.Queryable()
                    .Where(x => x.PROJECT_ID == model.ProjectId
                    && x.TIME_PERIOD_ID == model.PeriodId
                    && x.PROJECT_STRUCT_ID == model.ProjectStructId)
                    .FirstOrDefault();
                if (currentObj != null)
                {
                    CurrentRepository.Detach(currentObj);
                    currentObj.UPDATE_BY = currentUser;
                    currentObj.TOTAL = model.Total;
                    CurrentRepository.Update(currentObj);
                }
                else
                {
                    CurrentRepository.Create(new T_PS_PLAN_COST
                    {
                        ID = Guid.NewGuid(),
                        PROJECT_ID = model.ProjectId,
                        IS_CUSTOMER = model.IsCustomer,
                        PROJECT_STRUCT_ID = model.ProjectStructId,
                        TIME_PERIOD_ID = model.PeriodId,
                        CREATE_BY = currentUser,
                        TOTAL = model.Total
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

        internal void UpdateValue(UpdatePlanCostValueModel model)
        {
            try
            {
                var currentUser = ProfileUtilities.User?.USER_NAME;
                UnitOfWork.BeginTransaction();
                if (model.PeriodId == Guid.Empty)
                {
                    // update khoi luong thiet ke cua hang muc
                    UnitOfWork.Repository<ProjectStructRepo>().Queryable().Where(x => x.ID == model.ProjectStructId).Update(x => new T_PS_PROJECT_STRUCT
                    {
                        PLAN_VOLUME = model.Value,
                        UPDATE_BY = currentUser
                    });
                }
                else
                {
                    // update khoi luong trong ky
                    var currentObj = CurrentRepository.Queryable()
                        .Where(x => x.PROJECT_ID == model.ProjectId
                        && x.TIME_PERIOD_ID == model.PeriodId
                        && x.PROJECT_STRUCT_ID == model.ProjectStructId)
                        .FirstOrDefault();
                    if (currentObj != null)
                    {
                        CurrentRepository.Detach(currentObj);
                        currentObj.VALUE = model.Value;
                        currentObj.UPDATE_BY = currentUser;
                        currentObj.TOTAL = model.Total;
                        CurrentRepository.Update(currentObj);
                    }
                    else
                    {
                        CurrentRepository.Create(new T_PS_PLAN_COST
                        {
                            ID = Guid.NewGuid(),
                            PROJECT_ID = model.ProjectId,
                            IS_CUSTOMER = model.IsCustomer,
                            PROJECT_STRUCT_ID = model.ProjectStructId,
                            TIME_PERIOD_ID = model.PeriodId,
                            CREATE_BY = currentUser,
                            VALUE = model.Value,
                            TOTAL = model.Total
                        });
                    }
                }
                UnitOfWork.Repository<ProjectRepo>().ResetStatus(model.ProjectId, currentUser, model.IsCustomer ? "Kế hoạch sản lượng" : "Kế hoạch chi phí");
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        internal void SetHeaderExportExcel(ref MemoryStream outFileHeaderStream, MemoryStream outFileStream, Guid projectId, int startCol)
        {
            ObjDetail.PROJECT_ID = projectId;
            var projectTimes = GetProjectTime();
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
                int numberPeriods = projectTimes.Count;

                ReportUtilities.CreateCell(ref rowHeaderPeriod, numberPeriods * COL_PER_PERIOD + startCol + 1);
                ReportUtilities.CreateCell(ref rowHeaderValue, numberPeriods * COL_PER_PERIOD + startCol + 1);
                for (int index = 0; index < numberPeriods - 2; index++)
                {
                    var timePeriod = projectTimes[index];
                    if (index == 0)
                    {
                        rowHeaderPeriod.Cells[startCol - COL_PER_PERIOD + 1].SetCellValue($"{timeType.ToUpper()} {index + 1} ({timePeriod.START_DATE.ToString(Global.DateToStringFormat)}-{timePeriod.FINISH_DATE.ToString(Global.DateToStringFormat)})");
                    } else
                    {
                        int i = index - 1;
                        var cra = new CellRangeAddress(firstRow: 6, lastRow: 6, firstCol: i * COL_PER_PERIOD + startCol + 1, lastCol: i * COL_PER_PERIOD + startCol + COL_PER_PERIOD);
                        sheet.AddMergedRegion(cra);
                        for (int j = 1; j <= COL_PER_PERIOD; j++)
                        {
                            rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + j, i * COL_PER_PERIOD + startCol + j);
                            rowHeaderValue.Cells[startCol - COL_PER_PERIOD + j].CopyCellTo(i * COL_PER_PERIOD + startCol + j);
                            sheet.SetColumnWidth(i * COL_PER_PERIOD + startCol + j, sheet.GetColumnWidth(startCol - COL_PER_PERIOD + j));
                        }
                        rowHeaderPeriod.CopyCell(startCol - COL_PER_PERIOD + 1, i * COL_PER_PERIOD + startCol + 1).SetCellValue($"{timeType.ToUpper()} {index + 1} ({timePeriod.START_DATE.ToString(Global.DateToStringFormat)}-{timePeriod.FINISH_DATE.ToString(Global.DateToStringFormat)})");
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
