using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_AD_MESSAGE_Map : BaseMapping<T_AD_MESSAGE>
    {
        public T_AD_MESSAGE_Map()
        {
            Table("T_AD_MESSAGE");
            Id(x => x.PKID);
            Map(x => x.CODE).Not.Nullable();
            Map(x => x.LANGUAGE).Not.Nullable();
            Map(x => x.MESSAGE).Not.Nullable();
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
