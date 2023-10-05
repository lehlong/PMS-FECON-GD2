using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_AD_USER_USER_GROUP_Map : BaseMapping<T_AD_USER_USER_GROUP>
    {
        public T_AD_USER_USER_GROUP_Map()
        {
            Table("T_AD_USER_USER_GROUP");
            CompositeId()
                .KeyProperty(x => x.USER_NAME)
                .KeyProperty(x => x.USER_GROUP_CODE);
            References(x => x.User).Column("USER_NAME").Not.Insert().Not.Update().LazyLoad();
            References(x => x.UserGroup).Column("USER_GROUP_CODE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
