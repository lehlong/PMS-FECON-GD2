using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_ACTION_WORKFLOW_Map : BaseMapping<T_MD_ACTION_WORKFLOW>
    {
        public T_MD_ACTION_WORKFLOW_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.ACTIVE);
        }
    }
}
