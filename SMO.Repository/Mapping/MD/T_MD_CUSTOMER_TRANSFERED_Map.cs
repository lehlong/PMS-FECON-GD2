using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.PO
{
    public class T_MD_CUSTOMER_OLD_TRANSFERED_Map : BaseMapping<T_MD_CUSTOMER_OLD_TRANSFERED>
    {
        public T_MD_CUSTOMER_OLD_TRANSFERED_Map()
        {
            Table("T_MD_CUSTOMER_OLD_TRANSFERED");
            Id(x => x.PKID);
            Map(x => x.CUSTOMER_CODE_SOURCE);
            Map(x => x.COMPANY_CODE_SOURCE);
            Map(x => x.CUSTOMER_CODE_DES);
            Map(x => x.COMPANY_CODE_DES);
            References(x => x.CustomerSource).Columns("CUSTOMER_CODE_SOURCE", "COMPANY_CODE_SOURCE").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
            References(x => x.CustomerDes).Columns("CUSTOMER_CODE_DES", "COMPANY_CODE_DES").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
        }
    }
}
