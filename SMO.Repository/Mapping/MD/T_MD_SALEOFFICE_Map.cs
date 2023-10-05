using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_SALEOFFICE_Map : BaseMapping<T_MD_SALEOFFICE>
    {
        public T_MD_SALEOFFICE_Map()
        {
            Table("T_MD_SALEOFFICE");
            Id(x => x.CODE);
            Map(x => x.TEXT);
            Map(x => x.COMPANY_CODE);
            Map(x => x.DESCRIPTION);
            Map(x => x.DISCHARD_POINT);
            Map(x => x.EMAIL);
            Map(x => x.PHONE);
            Map(x => x.ACTIVE).Not.Nullable().CustomType<YesNoType>();
        }
    }
}
