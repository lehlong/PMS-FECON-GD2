using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_DISCHARD_Map : BaseMapping<T_MD_DISCHARD>
    {
        public T_MD_DISCHARD_Map()
        {
            Table("T_MD_DISCHARD");
            Id(x => x.CODE);
            Map(x => x.TEXT);
            Map(x => x.CUSTOMER_CODE);
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
