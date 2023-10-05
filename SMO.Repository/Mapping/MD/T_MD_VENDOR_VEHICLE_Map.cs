using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_VENDOR_VEHICLE_Map : BaseMapping<T_MD_VENDOR_VEHICLE>
    {
        public T_MD_VENDOR_VEHICLE_Map()
        {
            Table("T_MD_VENDOR_VEHICLE");
            CompositeId()
                .KeyProperty(x => x.VEHICLE_CODE)
                .KeyProperty(x => x.VENDOR_CODE);

            References(x => x.Vendor).Column("VENDOR_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Vehicle).Column("VEHICLE_CODE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
