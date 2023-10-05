using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_COMMENT_Map : BaseMapping<T_CM_COMMENT>
    {
        public T_CM_COMMENT_Map()
        {
            Table("T_CM_COMMENT");
            Id(x => x.CODE);
            Map(x => x.REFRENCE_ID).Not.Nullable();
            Map(x => x.CONTENTS).Not.Nullable();
        }
    }
}
