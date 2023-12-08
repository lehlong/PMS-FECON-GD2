using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PROJECT_WORKFLOW_COMMENT_Map : BaseMapping<T_PS_PROJECT_WORKFLOW_COMMENT>
    {
        public T_PS_PROJECT_WORKFLOW_COMMENT_Map()
        {
            Id(x => x.ID);
            Map(x => x.STEP_ID);
            Map(x => x.COMMENT);
            Map(x => x.ACTIVE);
        }
    }
}
