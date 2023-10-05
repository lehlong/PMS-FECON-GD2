using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_VOLUME_PROGRESS_HISTORY_Map : BaseMapping<T_PS_VOLUME_PROGRESS_HISTORY>
    {
        public T_PS_VOLUME_PROGRESS_HISTORY_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_ID).Not.Nullable().Not.Update();
            Map(x => x.RESOURCE_ID).Not.Nullable().Not.Update();
            Map(x => x.DES_STATUS).Not.Nullable().Not.Update();
            Map(x => x.PRE_STATUS).Not.Nullable().Not.Update();
            Map(x => x.ACTION).Not.Nullable().Not.Update();
            Map(x => x.ACTOR).Not.Nullable().Not.Update();
            Map(x => x.NOTE).Not.Update();
            Map(x => x.IS_CUSTOMER).Not.Nullable().Not.Update();
            Map(x => x.IS_ACCEPT).Not.Nullable().Not.Update();
        }
    }
}
