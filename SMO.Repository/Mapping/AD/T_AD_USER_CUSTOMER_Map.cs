using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_AD_USER_CUSTOMER_Map : BaseMapping<T_AD_USER_CUSTOMER>
    {
        public T_AD_USER_CUSTOMER_Map()
        {
            Table("T_AD_USER_CUSTOMER");
            CompositeId()
                .KeyProperty(x => x.CUSTOMER_CODE)
                .KeyProperty(x => x.USER_NAME)
                .KeyProperty(x => x.COMPANY_CODE);
            References(x => x.Customer).Columns("CUSTOMER_CODE", "COMPANY_CODE").Not.Insert().Not.Update().LazyLoad();
            //                           .Not.Insert().Not.Update();
            //References(x => x.Customer).Columns("COMPANY_CODE", "CUSTOMER_CODE").PropertyRef()
            //References(x => x.Customer).Columns("COMPANY_CODE", "CUSTOMER_CODE").PropertyRef(y => y.CompositeKey);
        }
    }
}
