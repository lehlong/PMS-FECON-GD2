using SMO.Core.Entities;
using SMO.Core.Entities.MD;

using System;

namespace SMO.Core.Common
{
    public class BaseProjectVolumeEntity : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual Guid TIME_PERIOD_ID { get; set; }
        public virtual DateTime? FROM_DATE { get; set; }
        public virtual DateTime? TO_DATE { get; set; }
        public virtual string STATUS { get; set; }
        public virtual string NOTES { get; set; }
        public virtual bool IS_CUSTOMER { get; set; }
        public virtual int UPDATE_TIMES { get; set; }
        public virtual string VENDOR_CODE { get; set; }
        public virtual string USER_XAC_NHAN { get; set; }
        public virtual string USER_PHE_DUYET { get; set; }
        public virtual string SAP_DOCID { get; set; }
        public virtual T_MD_VENDOR Vendor { get; set; }
    }
}
