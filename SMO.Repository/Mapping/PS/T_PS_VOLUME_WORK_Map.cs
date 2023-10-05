using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_VOLUME_WORK_Map : BaseMapping<T_PS_VOLUME_WORK>
    {
        public T_PS_VOLUME_WORK_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.IS_CUSTOMER).Not.Update();
            Map(x => x.NOTES);
            Map(x => x.PROJECT_ID).Not.Update();
            Map(x => x.STATUS);
            Map(x => x.TIME_PERIOD_ID);
            Map(x => x.FROM_DATE);
            Map(x => x.TO_DATE);
            Map(x => x.USER_XAC_NHAN);
            Map(x => x.USER_PHE_DUYET);
            Map(x => x.SAP_DOCID);
            Map(x => x.VENDOR_CODE).Not.Update();
            Map(x => x.UPDATE_TIMES).Not.Update();

            HasMany(x => x.Details).KeyColumn("HEADER_ID").Inverse().Cascade.All();

        }
    }
}
