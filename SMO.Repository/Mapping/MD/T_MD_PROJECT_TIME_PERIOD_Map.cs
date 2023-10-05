using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PROJECT_TIME_PERIOD_Map : BaseMapping<T_MD_PROJECT_TIME_PERIOD>
    {
        public T_MD_PROJECT_TIME_PERIOD_Map()
        {
            Id(x => x.CODE);
            Map(x => x.TEXT);
            Map(x => x.ACTIVE);
        }
    }
}
