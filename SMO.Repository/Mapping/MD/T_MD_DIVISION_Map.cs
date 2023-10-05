using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_DIVISION_Map : BaseMapping<T_MD_DIVISION>
    {
        public T_MD_DIVISION_Map()
        {
            Table("T_MD_DIVISION");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
            Map(x => x.COMPANY_CODE).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
