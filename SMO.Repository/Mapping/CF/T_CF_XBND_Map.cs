using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_XBND_Map : BaseMapping<T_CF_XBND>
    {
        public T_CF_XBND_Map()
        {
            Table("T_CF_XBND");
            Id(x => x.PKID);
            Map(x => x.DESCRIPTION);
            Map(x => x.COMPANY_CODE);
            Map(x => x.CUSTOMER_CODE);
            Map(x => x.DOC_TYPE);
            Map(x => x.SALES_ORG);
            Map(x => x.PLANT_CODE);
            Map(x => x.BATCH_CODE);
            Map(x => x.STORE_LOC);
            Map(x => x.SHPOINT_CODE);
            Map(x => x.VENDOR_CODE);
            Map(x => x.TRANSMODE_CODE);
            Map(x => x.SHTYPE_CODE);
            Map(x => x.POTYPE);
            Map(x => x.ROUTE_CODE);
        }
    }
}
