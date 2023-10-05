using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_TASK_CHECK_LIST_Map : BaseMapping<T_PS_TASK_CHECK_LIST>
    {
        public T_PS_TASK_CHECK_LIST_Map()
        {
            Id(x => x.ID);
            Map(x => x.STATUS);
            Map(x => x.TEXT);
            Map(x => x.TASK_ID).Not.Update();
        }
    }
}
