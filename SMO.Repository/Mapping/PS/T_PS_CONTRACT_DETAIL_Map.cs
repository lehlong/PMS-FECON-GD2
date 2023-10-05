using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_CONTRACT_DETAIL_Map : BaseMapping<T_PS_CONTRACT_DETAIL>
    {
        public T_PS_CONTRACT_DETAIL_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_STRUCT_ID);
            Map(x => x.CONTRACT_ID);
            Map(x => x.UNIT_CODE);
            Map(x => x.UNIT_PRICE);
            Map(x => x.VOLUME);
            Map(x => x.ACTIVE);
            Map(x => x.STATUS);

            References(x => x.Contract).Columns("CONTRACT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Unit).Columns("UNIT_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Struct).Columns("PROJECT_STRUCT_ID").Not.Insert().Not.Update().LazyLoad();

        }
    }
}
