using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_CONTRACT_FILE_Map : BaseMapping<T_PS_CONTRACT_FILE>
    {
        public T_PS_CONTRACT_FILE_Map()
        {
            Id(x => x.ID);
            Map(x => x.CONTRACT_ID);
            Map(x => x.FILE_EXTENSION);
            Map(x => x.FILE_ID);
            Map(x => x.FILE_NAME);
            Map(x => x.FILE_SIZE);
        }
    }
}
