using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_USER_RIGHT_Map : BaseMapping<T_AD_USER_RIGHT>
    {
        public T_AD_USER_RIGHT_Map()
        {
            Table("T_AD_USER_RIGHT");
            CompositeId()
                .KeyProperty(x => x.USER_NAME)
                .KeyProperty(x => x.FK_RIGHT);
            Map(x => x.IS_ADD).Not.Nullable().CustomType<YesNoType>();
            Map(x => x.IS_REMOVE).Not.Nullable().CustomType<YesNoType>();

        }
    }
}
