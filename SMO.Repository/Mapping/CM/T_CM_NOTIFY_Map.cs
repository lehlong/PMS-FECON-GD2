using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_NOTIFY_Map : BaseMapping<T_CM_NOTIFY>
    {
        public T_CM_NOTIFY_Map()
        {
            Table("T_CM_NOTIFY");
            Id(x => x.PKID);
            Map(x => x.USER_NAME);
            Map(x => x.CONTENTS);
            Map(x => x.RAW_CONTENTS);
            Map(x => x.IS_REAED).CustomType<YesNoType>();
            Map(x => x.IS_COUNTED).CustomType<YesNoType>();
        }
    }
}
