namespace SMO.Service.PS.ValidateImport
{
    public class ValidateMessage
    {
        public int Row { get; set; }
        public bool Status { get; set; }
        public ValidateErrorEnum Type { get; set; }
    }

    public enum ValidateErrorEnum
    {
        LEVEL_1_ERROR,
        UNIT_ERROR,
        ACTIVITY_CODE_ERROR,
        WBS_CODE_ERROR,
        FORMAT_DATE_ERROR,
        STRUCTURE_TYPE_ERROR,
        REQUIRED_FIELD_ERROR,
        PARENT_CODE_ERROR,
        PRICE_ERROR,
        DUPLICATE_CODE_ERROR
    }
}
