using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CUSTOMER_OLD_Map : BaseMapping<T_MD_CUSTOMER>
    {
        public T_MD_CUSTOMER_OLD_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.MST);
            Map(x => x.EMAIL);
            Map(x => x.PHONE);
            Map(x => x.ADDRESS);
            Map(x => x.SHORT_NAME);
        }
    }
}
