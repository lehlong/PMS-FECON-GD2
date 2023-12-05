using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS_PROJECT
{
    public class T_PS_PROJECT_WORKFLOW_FILE_Map : BaseMapping<T_PS_PROJECT_WORKFLOW_FILE>
    {
        public T_PS_PROJECT_WORKFLOW_FILE_Map()
        {
            Id(x => x.ID);
            Map(x => x.NAME);
            Map(x => x.WORKFLOW_CODE);
            Map(x => x.C_ORDER);
            Map(x => x.PROJECT_ID);
        }
    }
}
