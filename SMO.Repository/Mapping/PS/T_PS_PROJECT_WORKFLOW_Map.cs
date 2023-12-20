using SMO.Core.Entities.MD;
using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_PROJECT_WORKFLOW_Map : BaseMapping<T_PS_PROJECT_WORKFLOW>
    {
        public T_PS_PROJECT_WORKFLOW_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.CODE);
            Map(x => x.PROJECT_ID);
            Map(x => x.NAME);
            Map(x => x.REQUEST_TYPE_CODE);
            Map(x => x.ACTIVE);
            Map(x => x.PROJECT_LEVEL_CODE);
            Map(x => x.CONTRACT_VALUE_MIN);
            Map(x => x.CONTRACT_VALUE_MAX);
            Map(x => x.PURCHASE_TYPE_CODE);
            Map(x => x.AUTHORITY);
            References(x => x.ProjectLevel).Columns("PROJECT_LEVEL_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.RequestType).Columns("REQUEST_TYPE_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.PurchaseType).Columns("PURCHASE_TYPE_CODE").Not.Insert().Not.Update().LazyLoad();
            HasMany(x => x.ListFiles).KeyColumn("WORKFLOW_ID").LazyLoad().Inverse().Cascade.Delete();
            HasMany(x => x.ListSteps).KeyColumn("WORKFLOW_ID").LazyLoad().Inverse().Cascade.Delete();
        }
    }
}
