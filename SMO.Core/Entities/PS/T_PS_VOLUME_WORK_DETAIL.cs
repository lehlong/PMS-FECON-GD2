using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_VOLUME_WORK_DETAIL : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid HEADER_ID { get; set; }
        public virtual Guid REFERENCE_FILE_ID { get; set; }
        public virtual Guid PROJECT_STRUCT_ID { get; set; }
        public virtual decimal VALUE { get; set; }
        public virtual decimal PRICE { get; set; }
        public virtual decimal TOTAL { get; set; }
        public virtual decimal CONFIRM_VALUE { get; set; }
        public virtual string NOTES { get; set; }
    }
}
