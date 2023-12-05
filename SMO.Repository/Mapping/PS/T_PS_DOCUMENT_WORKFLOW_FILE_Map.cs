using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS_DOCUMENT
{
    public class T_PS_DOCUMENT_WORKFLOW_FILE_Map : BaseMapping<T_PS_DOCUMENT_WORKFLOW_FILE>
    {
        public T_PS_DOCUMENT_WORKFLOW_FILE_Map()
        {
            Id(x => x.ID);
            Map(x => x.NAME);
            Map(x => x.WORKFLOW_CODE);
            Map(x => x.C_ORDER);
            Map(x => x.DOCUMENT_ID);
            Map(x => x.PROJECT_ID);
        }
    }
}
