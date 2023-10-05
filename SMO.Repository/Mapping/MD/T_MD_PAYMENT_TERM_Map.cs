using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PAYMENT_TERM_Map : BaseMapping<T_MD_PAYMENT_TERM>
    {
        public T_MD_PAYMENT_TERM_Map()
        {
            Table("T_MD_PAYMENT_TERM");
            Id(x => x.CODE);
            Map(x => x.TEXT).Nullable();
            Map(x => x.PAYMENT_TERM_D).Nullable();
            Map(x => x.PAYMENT_TERM_M).Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
