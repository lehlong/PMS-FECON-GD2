using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PROJECT_PROFILE_Map : BaseMapping<T_MD_PROJECT_PROFILE>
    {
        public T_MD_PROJECT_PROFILE_Map()
        {
            Id(x => x.ID);
            Map(x => x.COMPANY_CODE);
            Map(x => x.PROJECT_PROFILE);
            Map(x => x.PROJECT_TYPE);
            Map(x => x.FIRST_CHARACTER);
        }
    }
}
