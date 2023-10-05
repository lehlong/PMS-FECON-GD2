using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_XTHG_Map : BaseMapping<T_CF_XTHG>
    {
        public T_CF_XTHG_Map()
        {
            Table("T_CF_XTHG");
            Id(x => x.PKID);
            Map(x => x.DESCRIPTION);
            Map(x => x.COMPANY_CODE);
            Map(x => x.CUSTOMER_CODE);
            Map(x => x.DOC_TYPE);
            Map(x => x.DC_CODE);
            Map(x => x.DIVISION_CODE);
            Map(x => x.SALES_ORG);
            Map(x => x.PLANT_CODE);
            Map(x => x.BATCH_CODE);
            Map(x => x.STORE_LOC);
            Map(x => x.SHPOINT_CODE);
            Map(x => x.VENDOR_CODE);
            Map(x => x.TRANSMODE_CODE);
        }
    }
}
