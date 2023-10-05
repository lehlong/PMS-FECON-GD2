using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CONTRACT_STATUS_Map : BaseMapping<T_MD_CONTRACT_STATUS>
    {
        public T_MD_CONTRACT_STATUS_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
        }
    }
}
