using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PROJECT_LEVEL_Map : BaseMapping<T_MD_PROJECT_LEVEL>
    {
        public T_MD_PROJECT_LEVEL_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.VALUE_FROM);
            Map(x => x.VALUE_TO);
            Map(x => x.THOI_GIAN);
            Map(x => x.NOTES);
        }
    }
}
