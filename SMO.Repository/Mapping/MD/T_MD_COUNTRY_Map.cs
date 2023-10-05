using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_COUNTRY_Map : BaseMapping<T_MD_COUNTRY>
    {
        public T_MD_COUNTRY_Map()
        {
            Table("T_MD_COUNTRY");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
        }
    }
}
