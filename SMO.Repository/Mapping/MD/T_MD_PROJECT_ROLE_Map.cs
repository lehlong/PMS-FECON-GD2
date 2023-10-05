using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PROJECT_ROLE_Map : BaseMapping<T_MD_PROJECT_ROLE>
    {
        public T_MD_PROJECT_ROLE_Map()
        {
            Id(x => x.ID);
            Map(x => x.NAME);
            Map(x => x.ACTIVE);
        }
    }
}
