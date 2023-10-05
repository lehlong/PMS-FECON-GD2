using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_REFERENCE_FILE_Map : BaseMapping<T_CM_REFERENCE_FILE>
    {
        public T_CM_REFERENCE_FILE_Map()
        {
            Table("T_CM_REFERENCE_FILE");
            CompositeId()
                .KeyProperty(x => x.REFERENCE_ID)
                .KeyProperty(x => x.FILE_ID);
            References(x => x.FileUpload).Column("FILE_ID").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
