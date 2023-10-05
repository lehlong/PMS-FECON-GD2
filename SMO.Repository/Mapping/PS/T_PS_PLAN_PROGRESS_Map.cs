using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PLAN_PROGRESS_Map : BaseMapping<T_PS_PLAN_PROGRESS>
    {
        public T_PS_PLAN_PROGRESS_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_STRUCT_ID);
            Map(x => x.CONTRACT_DETAIL_ID);
            Map(x => x.PROJECT_ID);
            Map(x => x.IS_CUSTOMER);
            Map(x => x.VALUE);

            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
            References(x => x.ProjectStruct).Columns("PROJECT_STRUCT_ID").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
            References(x => x.ContractDetail).Columns("CONTRACT_DETAIL_ID").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();

        }
    }
}
