using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CF_DCCH : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string DESCRIPTION { get; set; }
        public virtual string COMPANY_CODE { get; set; }
        public virtual string SALEOFFICE_CODE { get; set; }
        public virtual string DOC_TYPE { get; set; }
        public virtual string PO_ORG { get; set; }
        public virtual string PO_GROUP { get; set; }
        public virtual string PLANT_CODE { get; set; }
        public virtual string SUPPLY_PLANT_CODE { get; set; }
        public virtual string BATCH_CODE { get; set; }
        public virtual string VALUATION_CODE { get; set; }
        public virtual string STORE_LOC { get; set; }
        public virtual string SHPOINT_CODE { get; set; }
        public virtual string VENDOR_CODE { get; set; }
        public virtual string TRANSMODE_CODE { get; set; }
        public virtual string SHTYPE_CODE { get; set; }
        public virtual string ROUTE_CODE { get; set; }

        public virtual T_MD_SALEOFFICE SaleOffice { get; set; }
        //public virtual T_MD_ROUTE Route { get; set; }
        //public virtual T_MD_BATCH Batch { get; set; }
        //public virtual T_MD_TRANSMODE Transmode { get; set; }
        //public virtual T_MD_VENDOR Vendor { get; set; }
        //public virtual T_MD_SHPOINT ShPoint { get; set; }
    }
}
