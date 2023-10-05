using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_VOLUME_ACCEPT_Map : BaseMapping<T_PS_VOLUME_ACCEPT>
    {
        public T_PS_VOLUME_ACCEPT_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.IS_CUSTOMER).Not.Update();
            Map(x => x.NOTES);
            Map(x => x.PROJECT_ID).Not.Update();
            Map(x => x.STATUS);
            Map(x => x.TIME_PERIOD_ID);
            Map(x => x.FROM_DATE);
            Map(x => x.TO_DATE);
            Map(x => x.UPDATE_TIMES).Not.Update();
            Map(x => x.VENDOR_CODE).Not.Update();
            Map(x => x.USER_XAC_NHAN);
            Map(x => x.USER_PHE_DUYET);
            HasMany(x => x.Details).KeyColumn("HEADER_ID").Inverse().Cascade.All();
        }
    }
}
