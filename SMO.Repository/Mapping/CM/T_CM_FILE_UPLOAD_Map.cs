using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_CM_FILE_UPLOAD_Map : BaseMapping<T_CM_FILE_UPLOAD>
    {
        public T_CM_FILE_UPLOAD_Map()
        {
            Table("T_CM_FILE_UPLOAD");
            Id(x => x.PKID).GeneratedBy.Assigned();
            Map(x => x.FILE_NAME);
            Map(x => x.FILE_OLD_NAME);
            Map(x => x.FILE_EXT);
            Map(x => x.FILE_SIZE);
            Map(x => x.CONNECTION_ID);
            Map(x => x.DATABASE_NAME);
            Map(x => x.DIRECTORY_PATH);
        }
    }
}
