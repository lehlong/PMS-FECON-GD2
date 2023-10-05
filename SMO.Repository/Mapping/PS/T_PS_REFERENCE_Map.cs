using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_REFERENCE_Map : BaseMapping<T_PS_REFERENCE>
    {
        public T_PS_REFERENCE_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_ID);
            Map(x => x.SOURCE_ID);
            Map(x => x.TARGET_ID);
            Map(x => x.TYPE);
        }
    }
}
