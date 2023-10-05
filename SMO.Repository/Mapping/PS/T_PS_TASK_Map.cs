using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_TASK_Map : BaseMapping<T_PS_TASK>
    {
        public T_PS_TASK_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_ID).Not.Update();
            Map(x => x.ACTIVITY_PARENT_ID).Not.Update();
            Map(x => x.TEXT);
            Map(x => x.START_DATE);
            Map(x => x.FINISH_DATE);
            Map(x => x.USER_PERFORMER);
            Map(x => x.USER_APPROVE);
            Map(x => x.DESCRIPTION);
            Map(x => x.PRIORITY);
            Map(x => x.STATUS);

            HasMany(x => x.CheckLists).KeyColumn("TASK_ID").Inverse().Cascade.All().LazyLoad();
            References(x => x.UserAprove).Columns("USER_APPROVE").Not.Insert().Not.Update();
            References(x => x.UserPerformer).Columns("USER_PERFORMER").Not.Insert().Not.Update();
        }
    }
}
