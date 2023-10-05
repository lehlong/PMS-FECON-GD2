using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_COMMENT_Map : BaseMapping<T_PS_COMMENT>
    {
        public T_PS_COMMENT_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_ID);
            Map(x => x.USER_NAME);
            Map(x => x.MESSENGER);
            Map(x => x.IS_FILE);
            Map(x => x.PATH_FILE);
            Map(x => x.MIME_TYPE);
        }
    }
}
