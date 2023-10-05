using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_ACTIVITY_Map : BaseMapping<T_PS_ACTIVITY>
    {
        public T_PS_ACTIVITY_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.ASSIGNED_TO);
            Map(x => x.ACTUAL_FINISH_DATE);
            Map(x => x.ACTUAL_START_DATE);
            Map(x => x.ACTUAL_VOLUME);
            Map(x => x.ACTUAL_VOLUME_UNIT_ID);
            Map(x => x.FINISH_DATE);
            Map(x => x.PLAN_VOLUME);
            Map(x => x.PLAN_VOLUME_UNIT_ID);
            Map(x => x.PROJECT_ID).Not.Update();
            Map(x => x.REFRENCE_ID);
            Map(x => x.BOQ_REFRENCE_ID);
            Map(x => x.START_DATE);
            Map(x => x.PEOPLE_RESPONSIBILITY);
            Map(x => x.STATUS);
            Map(x => x.TEXT);
            Map(x => x.CODE);
            Map(x => x.REFERENCE_FILE_ID).Not.Update();
            Map(x => x.TOTAL_FLOAT);
            Map(x => x.FREE_FLOAT);
            Map(x => x.PREDECESSOR);
            Map(x => x.SUCCESSOR);
            Map(x => x.RELATIONSHIP_TYPE);
            Map(x => x.PURCHARING_ORG);
            Map(x => x.PURCHARING_GROUP);
            Map(x => x.CONTROL_KEY);

            HasMany(x => x.Tasks).KeyColumn("ACTIVITY_PARENT_ID").Inverse().Cascade.All();
            References(x => x.ReferenceBoq).Columns("BOQ_REFRENCE_ID").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
        }
    }
}
