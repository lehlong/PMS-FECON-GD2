using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_DC_Map : BaseMapping<T_MD_DC>
    {
        public T_MD_DC_Map()
        {
            Table("T_MD_DC");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
            Map(x => x.COMPANY_CODE).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
