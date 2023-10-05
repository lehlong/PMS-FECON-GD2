using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PAYMENT_STATUS_Map : BaseMapping<T_MD_PAYMENT_STATUS>
    {
        public T_MD_PAYMENT_STATUS_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
        }
    }
}
