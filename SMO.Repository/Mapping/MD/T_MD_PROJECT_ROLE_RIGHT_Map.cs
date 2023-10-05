using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PROJECT_ROLE_RIGHT_Map : BaseMapping<T_MD_PROJECT_ROLE_RIGHT>
    {
        public T_MD_PROJECT_ROLE_RIGHT_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_ROLE_ID);
            Map(x => x.RIGHT_CODE);
        }
    }
}
