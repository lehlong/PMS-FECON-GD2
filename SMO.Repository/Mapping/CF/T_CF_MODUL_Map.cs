using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_MODUL_Map : BaseMapping<T_CF_MODUL>
    {
        public T_CF_MODUL_Map()
        {
            Table("T_CF_MODUL");
            Id(x => x.PKID);
            Map(x => x.COMPANY_CODE);
            Map(x => x.MODUL_TYPE);
        }
    }
}
