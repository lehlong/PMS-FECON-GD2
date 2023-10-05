using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_EMAIL_Map : BaseMapping<T_CM_EMAIL>
    {
        public T_CM_EMAIL_Map()
        {
            Table("T_CM_EMAIL");
            Id(x => x.PKID);
            Map(x => x.STATUS);
            Map(x => x.EMAIL);
            Map(x => x.SUBJECT);
            Map(x => x.CONTENTS).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.NUMBER_RETRY);
            Map(x => x.IS_SEND).CustomType<YesNoType>();
        }
    }
}
