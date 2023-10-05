using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CF_STORE_MATERIAL_Map : BaseMapping<T_CF_STORE_MATERIAL>
    {
        public T_CF_STORE_MATERIAL_Map()
        {
            Table("T_CF_STORE_MATERIAL");
            CompositeId()
                .KeyProperty(x => x.STORE_CODE)
                .KeyProperty(x => x.MATERIAL_CODE);
            References(x => x.Store).Column("STORE_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Material).Column("MATERIAL_CODE").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
