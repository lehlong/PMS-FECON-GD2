using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_SL_DT_Map : BaseMapping<T_PS_SL_DT>
    {
        public T_PS_SL_DT_Map()
        {
            Id(x => x.ID);
            Map(x => x.CRITERIA_CODE);
            Map(x => x.PROJECT_ID);
            Map(x => x.TIME_PERIOD_ID);
            Map(x => x.VALUE);

            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Criteria).Columns("CRITERIA_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.TimePeriod).Columns("TIME_PERIOD_ID").Not.Insert().Not.Update().LazyLoad();

        }
    }
}
