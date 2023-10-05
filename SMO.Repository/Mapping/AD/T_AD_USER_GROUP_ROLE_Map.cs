using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_AD_USER_GROUP_ROLE_Map : BaseMapping<T_AD_USER_GROUP_ROLE>
    {
        public T_AD_USER_GROUP_ROLE_Map()
        {
            Table("T_AD_USER_GROUP_ROLE");
            CompositeId()
                .KeyProperty(x => x.ROLE_CODE)
                .KeyProperty(x => x.USER_GROUP_CODE);
            References(x => x.Role).Column("ROLE_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.UserGroup).Column("USER_GROUP_CODE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
