using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PLAN_COST_Map : BaseMapping<T_PS_PLAN_COST>
    {
        public T_PS_PLAN_COST_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_STRUCT_ID);
            Map(x => x.CONTRACT_DETAIL_ID);
            Map(x => x.PROJECT_ID);
            Map(x => x.TIME_PERIOD_ID);
            Map(x => x.VALUE);
            Map(x => x.TOTAL);
            Map(x => x.IS_CUSTOMER);

            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.ProjectStruct).Columns("PROJECT_STRUCT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.TimePeriod).Columns("TIME_PERIOD_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.ContractDetail).Columns("CONTRACT_DETAIL_ID").Not.Insert().Not.Update().LazyLoad();

        }
    }
}
