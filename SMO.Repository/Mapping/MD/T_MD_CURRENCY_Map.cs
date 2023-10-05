using SMO.Core.Entities.MD;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CURRENCY_Map : BaseMapping<T_MD_CURRENCY>
    {
        public T_MD_CURRENCY_Map()
        {
            Id(x => x.CODE);
            Map(x => x.NAME);
        }
    }
}
