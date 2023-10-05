using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Entities.PS
{
    public class T_PS_RESOURCE_OTHER : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string FULL_NAME { get; set; }
        public virtual string VENDOR_CODE { get; set; }
        public virtual string CMT { get; set; }
        public virtual string PHONE { get; set; }
        public virtual string EMAIL { get; set; }
        public virtual string VAI_TRO { get; set; }
        public virtual DateTime FROM_DATE { get; set; }
        public virtual DateTime TO_DATE { get; set; }
        public virtual string IS_DELETE { get; set; }

        public virtual DateTime? FILTER_FROM_DATE { get; set; }
        public virtual DateTime? FILTER_TO_DATE { get; set; }

        public virtual T_PS_PROJECT Project { get; set; }
    }
}
