using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_WORKFLOW_STEP_Map : BaseMapping<T_MD_WORKFLOW_STEP>
    {
        public T_MD_WORKFLOW_STEP_Map()
        {
            Id(x => x.ID);
            Map(x => x.WORKFLOW_CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE);
            Map(x => x.PROJECT_ROLE_CODE);
            Map(x => x.USER_ACTION);
            Map(x => x.NUMBER_DAYS);
            Map(x => x.ACTION);
            Map(x => x.C_ORDER);
        }
    }
}
