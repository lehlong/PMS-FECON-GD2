using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_VEHICLE_COMPARTMENT_Map : BaseMapping<T_MD_VEHICLE_COMPARTMENT>
    {
        public T_MD_VEHICLE_COMPARTMENT_Map()
        {
            Table("T_MD_VEHICLE_COMPARTMENT");
            Id(x => x.CODE);
            Map(x => x.VEHICLE_CODE).Nullable();
            Map(x => x.SEQ_NUMBER).Nullable();
            Map(x => x.CAPACITY).Nullable();
        }
    }
}
