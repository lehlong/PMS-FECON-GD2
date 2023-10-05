using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CUSTOMER_OLD_OLD_Map : BaseMapping<T_MD_CUSTOMER_OLD>
    {
        public T_MD_CUSTOMER_OLD_OLD_Map()
        {
            Table("T_MD_CUSTOMER_OLD_OLD");
            CompositeId()
               .KeyProperty(x => x.CUSTOMER_CODE)
               .KeyProperty(x => x.COMPANY_CODE);

            Map(x => x.TEXT).Nullable();
            Map(x => x.ADDRESS).Nullable();
            Map(x => x.VAT_NUMBER).Nullable();
            Map(x => x.EMAIL).Nullable();
            Map(x => x.PHONE).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
            //Map(x => x.IS_SEND_ONLY_REJECT).CustomType<YesNoType>();
            //References(x => x.Credit).Columns("CUSTOMER_CODE", "COMPANY_CODE").Not.Insert().Not.Update().Not.ForeignKey();
        }
    }
}
