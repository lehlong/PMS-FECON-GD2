using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_UNIT_Map : BaseMapping<T_MD_UNIT>
    {
        public T_MD_UNIT_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.SKF);
        }
    }
}
