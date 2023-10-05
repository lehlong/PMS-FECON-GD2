using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.AD
{
    public class T_AD_CONNECTION_Map : BaseMapping<T_AD_CONNECTION>
    {
        public T_AD_CONNECTION_Map()
        {
            Table("T_AD_CONNECTION");
            Id(x => x.PKID);
            Map(x => x.NAME);
            Map(x => x.ADDRESS);
            Map(x => x.USERNAME);
            Map(x => x.PASSWORD);
            Map(x => x.DIRECTORY);
            Map(x => x.NOTES);
        }
    }
}
