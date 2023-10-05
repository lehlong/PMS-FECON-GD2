using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_ROLE_DETAIL_Map : BaseMapping<T_AD_ROLE_DETAIL>
    {
        public T_AD_ROLE_DETAIL_Map()
        {
            Table("T_AD_ROLE_DETAIL");
            CompositeId()
                .KeyProperty(x => x.FK_ROLE)
                .KeyProperty(x => x.FK_RIGHT);
        }
    }
}
