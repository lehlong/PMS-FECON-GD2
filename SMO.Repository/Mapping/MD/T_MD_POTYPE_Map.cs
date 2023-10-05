using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_POTYPE_Map : BaseMapping<T_MD_POTYPE>
    {
        public T_MD_POTYPE_Map()
        {
            Table("T_MD_POTYPE");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
