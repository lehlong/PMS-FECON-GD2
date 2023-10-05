using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_AD_USER_VENDOR_Map : BaseMapping<T_AD_USER_VENDOR>
    {
        public T_AD_USER_VENDOR_Map()
        {
            Table("T_AD_USER_VENDOR");
            CompositeId()
                .KeyProperty(x => x.VENDOR_CODE)
                .KeyProperty(x => x.USER_NAME);
            References(x => x.Vendor).Columns("VENDOR_CODE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
