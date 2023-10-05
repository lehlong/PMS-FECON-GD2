using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CONTRACT_TYPE_Map : BaseMapping<T_MD_CONTRACT_TYPE>
    {
        public T_MD_CONTRACT_TYPE_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
            Map(x => x.IS_CUSTOMER);
        }
    }
}
