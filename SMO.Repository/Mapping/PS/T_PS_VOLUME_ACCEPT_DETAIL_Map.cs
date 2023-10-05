using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_VOLUME_ACCEPT_DETAIL_Map : BaseMapping<T_PS_VOLUME_ACCEPT_DETAIL>
    {
        public T_PS_VOLUME_ACCEPT_DETAIL_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_STRUCT_ID).Not.Update();
            Map(x => x.HEADER_ID).Generated.Never().Not.Update();
            Map(x => x.REFERENCE_FILE_ID).Not.Update();
            Map(x => x.VALUE);
            Map(x => x.PRICE);
            Map(x => x.TOTAL);
            Map(x => x.NOTES);
            Map(x => x.CONFIRM_VALUE);
        }
    }
}
