using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;

using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SMO.Service.PS.ValidateImport
{
    public class ValidateImportProject : IValidateImportProject
    {
        public IEnumerable<ValidateMessage> ValidateData(DataTable data, string projectCode, bool isStructureCost, IEnumerable<string> unitCodes)
        {
            List<ValidateMessage> result = new List<ValidateMessage>();
            var codes = new Dictionary<int, string>();
            for (int i = 2; i < data.Rows.Count; i++)
            {
                var row = data.Rows[i];
                codes.Add(i, row[0]?.ToString());
                if (i == 2)
                {
                    result.Add(ValidateLevel1(row, projectCode));
                } else
                {
                    result.Add(ValidateParentCode(row, projectCode, i));
                }
                result.Add(ValidateUnit(row, i, unitCodes));

                if (isStructureCost)
                {
                    result.Add(ValidateWbsCode(row, projectCode, i));
                    result.Add(ValidateActivityCode(row, i));
                    result.Add(ValidateStructureType(row, new string[2] { ProjectEnum.ACTIVITY.ToString(), ProjectEnum.WBS.ToString() }, i));
                }
                else
                {
                    result.Add(ValidateStructureType(row, new string[1] { ProjectEnum.BOQ.ToString() }, i));

                }

                result.Add(ValidateDateFormat(row, i));
                result.Add(ValidateRequiredFields(row, i));
                result.Add(ValidatePriceError(row, i));
            }
            result.AddRange(ValidateDupplicateCode(codes));

            return result.Where(x => !x.Status).ToList();
        }

        private IEnumerable<ValidateMessage> ValidateDupplicateCode(Dictionary<int, string> codes)
        {
            var groupCodes = codes.GroupBy(x => x.Value);
            return from groupSameCode in groupCodes
                   where groupSameCode.Count() > 1
                   from dict in groupSameCode
                   select new ValidateMessage
                   {
                       Row = dict.Key,
                       Status = false,
                       Type = ValidateErrorEnum.DUPLICATE_CODE_ERROR
                   };
        }

        public ValidateMessage ValidatePriceError(DataRow row, int i)
        {
            decimal quantity;
            decimal price;
            var quantityStr = row[6].ToString();
            var priceStr = row[7].ToString();
            var canParseQuantity = decimal.TryParse(quantityStr, out quantity);
            var canParsePrice = decimal.TryParse(priceStr, out price); 
            var unit = row[8].ToString();
            bool status;
            if (string.IsNullOrEmpty(quantityStr))
            {
                status = string.IsNullOrEmpty(priceStr) || canParsePrice;
            } else
            {
                if (canParseQuantity)
                {
                    if (quantity == 0)
                    {
                        status = string.IsNullOrEmpty(priceStr) || canParsePrice;
                    } else
                    {
                        status = canParsePrice && !string.IsNullOrEmpty(unit);
                    }
                } else
                {
                    status = false;
                }
            }

            return new ValidateMessage
            {
                Status = status,
                Row = i,
                Type = ValidateErrorEnum.PRICE_ERROR
            };
        }

        public string GetValidateMessage(IEnumerable<ValidateMessage> validateResult, bool isStructureCost)
        {
            var validateGroup = validateResult.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.Select(y => y.Row));
            var message = new StringBuilder();
            foreach (var validate in validateGroup)
            {
                var rowExcels = validate.Value.ToList();
                var rows = from row in rowExcels
                           select row + 1;
                switch (validate.Key)
                {
                    case ValidateErrorEnum.LEVEL_1_ERROR:
                        var structureLevel1Type = isStructureCost ? "WBS" : "BOQ";
                        message.Append($"<p>Chưa có hạng mục {structureLevel1Type} cấp 1. Lỗi tại dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.UNIT_ERROR:
                        message.Append($"<p>Không có đơn vị tính trong danh mục (có phân biệt viết hoa/viết thường). Lỗi tại các dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.ACTIVITY_CODE_ERROR:
                        message.Append($"<p>Sai định dạng mã ACTIVITY, định dạng 4 kí tự. Lỗi tại các dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.WBS_CODE_ERROR:
                        message.Append($"<p>Sai mã WBS. Lỗi tại các dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.FORMAT_DATE_ERROR:
                        message.Append($"<p>Lỗi thiếu/sai định dạng format ngày. Lỗi tại các dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.STRUCTURE_TYPE_ERROR:
                        var suggestType = isStructureCost ? "WBS hoặc ACTIVITY" : "BOQ";
                        message.Append($"<p>Sai loại đối tượng, nhập là {suggestType}. Lỗi tại các dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.REQUIRED_FIELD_ERROR:
                        message.Append($"<p>Chưa nhập đủ thông tin bắt buộc. Lỗi tại các dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.PARENT_CODE_ERROR:
                        message.Append($"<p>Chưa nhập PARENT_CODE. Lỗi tại các dòng {string.Join(",", rows)}</p>");
                        break;
                    case ValidateErrorEnum.PRICE_ERROR:
                        message.Append($"<p>Thiếu thông tin đơn giá hoặc ĐVT. Lỗi tại các dòng {string.Join(", ", rows)}</p>");
                        break;
                    case ValidateErrorEnum.DUPLICATE_CODE_ERROR:
                        message.Append($"<p>Mã hạng mục bị trùng. Lỗi tại các dòng {string.Join(", ", rows)}</p>");
                        break;
                    default:
                        break;
                }
            }

            return message.ToString();
        }

        public ValidateMessage ValidateParentCode(DataRow row, string projectCode, int i)
        {
            var parentCode = row[1].ToString();
            var code = row[0].ToString();
            bool status;
            if (code == projectCode)
            {
                status = string.IsNullOrEmpty(parentCode);
            } else
            {
                status = !string.IsNullOrEmpty(parentCode);
            }

            return new ValidateMessage
            {
                Status = status,
                Row = i,
                Type = ValidateErrorEnum.PARENT_CODE_ERROR
            };
        }
        public ValidateMessage ValidateRequiredFields(DataRow row, int i)
        {
            var requiredFieldIndexes = new int[5] { 0, 2, 3, 4, 5, };
            foreach (var index in requiredFieldIndexes)
            {
                if (string.IsNullOrEmpty(row[index].ToString()))
                {
                    return new ValidateMessage
                    {
                        Status = false,
                        Row = i,
                        Type = ValidateErrorEnum.REQUIRED_FIELD_ERROR
                    };
                }
            }
            return new ValidateMessage
            {
                Status = true
            };
        }
        public ValidateMessage ValidateStructureType(DataRow row, string[] validTypes, int i)
        {
            var rowType = row[5].ToString();
            return new ValidateMessage
            {
                Status = validTypes.Contains(rowType),
                Row = i,
                Type = ValidateErrorEnum.STRUCTURE_TYPE_ERROR
            };
        }
        public ValidateMessage ValidateDateFormat(DataRow row, int i)
        {
            DateTime startDate;
            DateTime finishDate;
            var canParseStartDate = DateTime.TryParseExact(row[3].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate);
            var canParseFinishDate = DateTime.TryParseExact(row[4].ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out finishDate);
            return new ValidateMessage
            {
                Status = canParseFinishDate && canParseStartDate,
                Row = i,
                Type = ValidateErrorEnum.FORMAT_DATE_ERROR
            };
        }
        public ValidateMessage ValidateActivityCode(DataRow row, int i)
        {
            var type = row[5].ToString();
            var code = row[0].ToString();
            var status = true;
            int codeNumber;
            if (type == ProjectEnum.ACTIVITY.ToString())
            {
                status = int.TryParse(code, out codeNumber) && code.Length == 4;
            }
            return new ValidateMessage
            {
                Row = i,
                Status = status,
                Type = ValidateErrorEnum.ACTIVITY_CODE_ERROR
            };
        }
        public ValidateMessage ValidateWbsCode(DataRow row, string projectCode, int i)
        {
            var type = row[5].ToString();
            var code = row[0].ToString();
            var status = true;
            if (type == ProjectEnum.WBS.ToString())
            {
                status = code.StartsWith(projectCode);
            }
            return new ValidateMessage
            {
                Row = i,
                Status = status,
                Type = ValidateErrorEnum.WBS_CODE_ERROR
            };
        }
        public ValidateMessage ValidateUnit(DataRow row, int i, IEnumerable<string> unitCodes)
        {
            var unitCode = row[8]?.ToString();
            var status = string.IsNullOrEmpty(unitCode) || unitCodes.Contains(unitCode);

            return new ValidateMessage
            {
                Status = status,
                Row = i,
                Type = ValidateErrorEnum.UNIT_ERROR
            };
        }
        public ValidateMessage ValidateLevel1(DataRow row, string projectCode)
        {
            var code = row[0].ToString();
            var parentCode = row[1].ToString();
            var status = code == projectCode && string.IsNullOrEmpty(parentCode);
            return new ValidateMessage
            {
                Status = status,
                Row = 2,
                Type = ValidateErrorEnum.LEVEL_1_ERROR
            };
        }
    }
}
