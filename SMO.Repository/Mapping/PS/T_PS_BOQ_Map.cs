using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_BOQ_Map : BaseMapping<T_PS_BOQ>
    {
        public T_PS_BOQ_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.ACTUAL_VOLUME);
            Map(x => x.ACTUAL_VOLUME_UNIT_ID);
            Map(x => x.FINISH_DATE);
            Map(x => x.PLAN_VOLUME);
            Map(x => x.PLAN_VOLUME_UNIT_ID);
            Map(x => x.PROJECT_ID).Not.Update();
            Map(x => x.TEXT);
            Map(x => x.STATUS);
            Map(x => x.START_DATE);
            Map(x => x.PEOPLE_RESPONSIBILITY);
            Map(x => x.CODE);
            Map(x => x.REFERENCE_FILE_ID).Not.Update();
            Map(x => x.ACTUAL_FINISH_DATE);
            Map(x => x.ACTUAL_START_DATE);
        }
    }
}
