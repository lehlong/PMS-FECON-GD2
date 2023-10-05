using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_BATCH_Map : BaseMapping<T_MD_BATCH>
    {
        public T_MD_BATCH_Map()
        {
            Table("T_MD_BATCH");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
