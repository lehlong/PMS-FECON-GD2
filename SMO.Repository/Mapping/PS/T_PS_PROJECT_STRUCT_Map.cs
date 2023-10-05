using NHibernate.Type;
using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PROJECT_STRUCT_Map : BaseMapping<T_PS_PROJECT_STRUCT>
    {
        public T_PS_PROJECT_STRUCT_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.ACTIVITY_ID).Not.Update();
            Map(x => x.BOQ_ID).Not.Update();
            Map(x => x.C_ORDER);
            Map(x => x.FINISH_DATE);
            Map(x => x.PARENT_ID);
            Map(x => x.PROJECT_ID).Not.Nullable().Not.Update();
            Map(x => x.START_DATE);
            Map(x => x.STATUS);
            Map(x => x.TYPE).Not.Nullable().Not.Update();
            Map(x => x.TASK_ID).Not.Update();
            Map(x => x.TEXT).Not.Nullable();
            Map(x => x.WBS_ID).Not.Update();
            Map(x => x.ACTIVE);
            Map(x => x.GEN_CODE);
            Map(x => x.UNIT_CODE);
            Map(x => x.PRICE);
            Map(x => x.TOTAL);
            Map(x => x.PLAN_VOLUME);
            Map(x => x.QUANTITY);
            Map(x => x.IS_CREATE_ON_SAP).Not.Nullable().CustomType<YesNoType>();

            References(x => x.Parent).Columns("PARENT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Wbs).Columns("WBS_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Activity).Columns("ACTIVITY_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Boq).Columns("BOQ_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Unit).Columns("UNIT_CODE").Not.Insert().Not.Update().LazyLoad();

        }
    }
}
