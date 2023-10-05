using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_VEHICLE_Map : BaseMapping<T_MD_VEHICLE>
    {
        public T_MD_VEHICLE_Map()
        {
            Table("T_MD_VEHICLE");
            Id(x => x.CODE);
            Map(x => x.TRANSUNIT_CODE);
            Map(x => x.UNIT);
            Map(x => x.OIC_PBATCH);
            Map(x => x.OIC_PTRIP);
            Map(x => x.CAPACITY);
            Map(x => x.TRANSMODE_CODE);
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
            HasMany(x => x.ListCompartment).KeyColumn("VEHICLE_CODE").Inverse().Cascade.Delete();
        }
    }
}
