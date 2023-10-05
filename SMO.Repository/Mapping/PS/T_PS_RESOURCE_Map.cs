using NHibernate.Type;
using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_RESOURCE_Map : BaseMapping<T_PS_RESOURCE>
    {
        public T_PS_RESOURCE_Map()
        {
            Id(x => x.ID);
            Map(x => x.USER_NAME);
            Map(x => x.PROJECT_ID);
            Map(x => x.PROJECT_ROLE_ID);
            Map(x => x.PROJECT_USER_TYPE_CODE);
            Map(x => x.FROM_DATE);
            Map(x => x.TO_DATE);
            Map(x => x.IS_SEND_MAIL).Not.Nullable().CustomType<YesNoType>();

            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
            References(x => x.ProjectRole).Columns("PROJECT_ROLE_ID").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
            References(x => x.ProjectUserType).Columns("PROJECT_USER_TYPE_CODE").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
            References(x => x.User).Columns("USER_NAME").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();

        }
    }
}
