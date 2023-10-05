using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CF_MHGL : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string DESCRIPTION { get; set; }
        public virtual string COMPANY_CODE { get; set; }
        public virtual string CUSTOMER_CODE { get; set; }
        public virtual string DOC_TYPE { get; set; }
        public virtual string SALES_ORG { get; set; }
        public virtual string PLANT_CODE { get; set; }
        public virtual string BATCH_CODE { get; set; }
        public virtual string STORE_LOC { get; set; }
        public virtual string SHPOINT_CODE { get; set; }
        public virtual string VENDOR_CODE { get; set; }
        public virtual string TRANSMODE_CODE { get; set; }

        //public virtual T_MD_POTYPE PoType { get; set; }
        //public virtual T_MD_ROUTE Route { get; set; }
        //public virtual T_MD_BATCH Batch { get; set; }
        //public virtual T_MD_TRANSMODE Transmode { get; set; }
        //public virtual T_MD_VENDOR Vendor { get; set; }
        //public virtual T_MD_SHPOINT ShPoint { get; set; }
    }
}
