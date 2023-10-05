using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_TITLE_Map : BaseMapping<T_MD_TITLE>
    {
        public T_MD_TITLE_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE);
        }
    }
}
