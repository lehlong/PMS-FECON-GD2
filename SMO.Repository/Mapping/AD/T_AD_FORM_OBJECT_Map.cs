using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_FORM_OBJECT_Map : BaseMapping<T_AD_FORM_OBJECT>
    {
        public T_AD_FORM_OBJECT_Map()
        {
            Table("T_AD_FORM_OBJECT");
            Id(x => x.PKID);
            Map(x => x.FK_FORM).Not.Nullable();
            Map(x => x.OBJECT_CODE).Not.Nullable();
            Map(x => x.TYPE).Not.Nullable();
        }
    }
}
