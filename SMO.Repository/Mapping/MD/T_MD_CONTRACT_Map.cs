using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.MD
{
    public class T_MD_CONTRACT_Map : BaseMapping<T_MD_CONTRACT>
    {
        public T_MD_CONTRACT_Map()
        {
            Table("T_MD_CONTRACT");
            Id(x => x.CODE);
            Map(x => x.CONTRACT_TYPE).Nullable();
            Map(x => x.SALEORG).Nullable();
            Map(x => x.DC_CODE).Nullable();
            Map(x => x.DIVISION_CODE).Nullable();
            Map(x => x.CUSTOMER_CODE).Nullable();
            Map(x => x.INCOTERMS1).Nullable();
            Map(x => x.INCOTERMS2).Nullable();
            Map(x => x.PAYMENTTERM_CODE).Nullable();
            Map(x => x.VALID_FROM).Nullable();
            Map(x => x.VALID_TO).Nullable();
        }
    }
}
