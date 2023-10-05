using NHibernate.Type;
using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PROJECT_USER_RIGHT_Map : BaseMapping<T_PS_PROJECT_USER_RIGHT>
    {
        public T_PS_PROJECT_USER_RIGHT_Map()
        {
            Id(x => x.ID);
            Map(x => x.USER_NAME);
            Map(x => x.PROJECT_ID);
            Map(x => x.RIGHT_CODE);
            Map(x => x.IS_ADD).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_REMOVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
