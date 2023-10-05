using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_NUMBER_RANGE_Map : BaseMapping<T_MD_NUMBER_RANGE>
    {
        public T_MD_NUMBER_RANGE_Map()
        {
            Id(x => x.ID);
            Map(x => x.CURRENT_NUMBER);
            Map(x => x.CHARACTER);
        }
    }
}
