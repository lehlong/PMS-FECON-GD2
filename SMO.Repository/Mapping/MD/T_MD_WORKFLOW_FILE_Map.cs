using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_WORKFLOW_FILE_Map : BaseMapping<T_MD_WORKFLOW_FILE>
    {
        public T_MD_WORKFLOW_FILE_Map()
        {
            Id(x => x.ID);
            Map(x => x.NAME);
            Map(x => x.WORKFLOW_CODE);
            Map(x => x.C_ORDER);
        }
    }
}
