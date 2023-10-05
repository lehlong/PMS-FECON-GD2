using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_LANGUAGE_Map : BaseMapping<T_AD_LANGUAGE>
    {
        public T_AD_LANGUAGE_Map()
        {
            Table("T_AD_LANGUAGE");
            Id(x => x.PKID);
            Map(x => x.FK_CODE).Not.Nullable();
            Map(x => x.OBJECT_TYPE).Not.Nullable();
            Map(x => x.LANG).Not.Nullable();
            Map(x => x.VALUE).Not.Nullable();
            Map(x => x.FORM_CODE).Nullable();
        }
    }
}
