using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_WORKFLOW_Map : BaseMapping<T_MD_WORKFLOW>
    {
        public T_MD_WORKFLOW_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.REQUEST_TYPE_CODE);
            Map(x => x.ACTIVE);
            Map(x => x.PROJECT_LEVEL_CODE);
            Map(x => x.CONTRACT_VALUE_MIN);
            Map(x => x.CONTRACT_VALUE_MAX);
            Map(x => x.PURCHASE_TYPE_CODE);
            Map(x => x.AUTHORITY);
        }
    }
}
