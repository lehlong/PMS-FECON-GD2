using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_RESOURCE_OTHER_Map : BaseMapping<T_PS_RESOURCE_OTHER>
    {
        public T_PS_RESOURCE_OTHER_Map()
        {
            Id(x => x.ID);
            Map(x => x.FULL_NAME);
            Map(x => x.PROJECT_ID);
            Map(x => x.VENDOR_CODE);
            Map(x => x.CMT);
            Map(x => x.PHONE);
            Map(x => x.EMAIL);
            Map(x => x.VAI_TRO);
            Map(x => x.FROM_DATE);
            Map(x => x.TO_DATE);

            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad().NotFound.Ignore();
        }
    }
}
