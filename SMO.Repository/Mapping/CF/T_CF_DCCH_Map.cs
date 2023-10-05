using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_DCCH_Map : BaseMapping<T_CF_DCCH>
    {
        public T_CF_DCCH_Map()
        {
            Table("T_CF_DCCH");
            Id(x => x.PKID);
            Map(x => x.DESCRIPTION);
            Map(x => x.COMPANY_CODE);
            Map(x => x.SALEOFFICE_CODE);
            Map(x => x.DOC_TYPE);
            Map(x => x.PO_ORG);
            Map(x => x.PO_GROUP);
            Map(x => x.PLANT_CODE);
            Map(x => x.SUPPLY_PLANT_CODE);
            Map(x => x.BATCH_CODE);
            Map(x => x.VALUATION_CODE);
            Map(x => x.STORE_LOC);
            Map(x => x.SHPOINT_CODE);
            Map(x => x.VENDOR_CODE);
            Map(x => x.TRANSMODE_CODE);
            Map(x => x.SHTYPE_CODE);
            Map(x => x.ROUTE_CODE);
            References(x => x.SaleOffice).Columns("SALEOFFICE_CODE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
