using SMO.Core.Entities.MD;

using System.Collections.Generic;
using System.Data;

namespace SMO.Service.PS.ValidateImport
{
    public interface IValidateImportProject
    {
        ValidateMessage ValidateActivityCode(DataRow row, int i);
        IEnumerable<ValidateMessage> ValidateData(DataTable data, string projectCode, bool isStructureCost, IEnumerable<string> unitCodes);
        ValidateMessage ValidateDateFormat(DataRow row, int i);
        ValidateMessage ValidateParentCode(DataRow row, string projectCode, int i);
        ValidateMessage ValidatePriceError(DataRow row, int i);
        ValidateMessage ValidateRequiredFields(DataRow row, int i);
        ValidateMessage ValidateLevel1(DataRow row, string projectCode);
        ValidateMessage ValidateStructureType(DataRow row, string[] strings, int i);
        ValidateMessage ValidateUnit(DataRow row, int i, IEnumerable<string> units);
        ValidateMessage ValidateWbsCode(DataRow row, string projectCode, int i);
        string GetValidateMessage(IEnumerable<ValidateMessage> validateResult, bool isStructureCost);
    }
}