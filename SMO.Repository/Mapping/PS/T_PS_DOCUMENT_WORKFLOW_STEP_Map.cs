using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_DOCUMENT_WORKFLOW_STEP_Map : BaseMapping<T_PS_DOCUMENT_WORKFLOW_STEP>
    {
        public T_PS_DOCUMENT_WORKFLOW_STEP_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.WORKFLOW_CODE);
            Map(x => x.WORKFLOW_ID);
            Map(x => x.NAME);
            Map(x => x.ACTIVE);
            Map(x => x.PROJECT_ROLE_CODE);
            Map(x => x.USER_ACTION);
            Map(x => x.NUMBER_DAYS);
            Map(x => x.ACTION);
            Map(x => x.C_ORDER);
            Map(x => x.IS_DONE);
            Map(x => x.PROJECT_ID);
            Map(x => x.DOCUMENT_ID);

        }
    }
}
