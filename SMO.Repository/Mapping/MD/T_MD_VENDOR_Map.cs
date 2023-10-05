using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_VENDOR_Map : BaseMapping<T_MD_VENDOR>
    {
        public T_MD_VENDOR_Map()
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
