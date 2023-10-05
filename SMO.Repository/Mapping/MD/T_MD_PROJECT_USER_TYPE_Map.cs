using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PROJECT_USER_TYPE_Map : BaseMapping<T_MD_PROJECT_USER_TYPE>
    {
        public T_MD_PROJECT_USER_TYPE_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE);
        }
    }
}
