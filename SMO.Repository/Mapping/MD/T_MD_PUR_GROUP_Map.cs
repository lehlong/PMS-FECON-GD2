using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_PUR_GROUP_Map : BaseMapping<T_MD_PUR_GROUP>
    {
        public T_MD_PUR_GROUP_Map()
        {
            Id(x => x.CODE);
            Map(x => x.COMPANY_CODE);
            Map(x => x.NAME);
        }
    }
}
