using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS_DOCUMENT
{
    public class T_PS_DOCUMENT_WORKFLOW_COMMENT_Map : BaseMapping<T_PS_DOCUMENT_WORKFLOW_COMMENT>
    {
        public T_PS_DOCUMENT_WORKFLOW_COMMENT_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.STEP_ID);
            Map(x => x.COMMENT);
        }
    }
}
