using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PLAN_VOLUME_DESIGN_Map : BaseMapping<T_PS_PLAN_VOLUME_DESIGN>
    {
        public T_PS_PLAN_VOLUME_DESIGN_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_STRUCT_ID);
            Map(x => x.CONTRACT_DETAIL_ID);
            Map(x => x.PROJECT_ID);
            Map(x => x.VALUE);
        }
    }
}
