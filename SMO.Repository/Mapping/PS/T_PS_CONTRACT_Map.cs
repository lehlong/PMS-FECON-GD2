using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_CONTRACT_Map : BaseMapping<T_PS_CONTRACT>
    {
        public T_PS_CONTRACT_Map()
        {
            Id(x => x.ID);
            Map(x => x.CONTRACT_NUMBER);
            Map(x => x.CONTRACT_TYPE);
            Map(x => x.CONTRACT_VALUE);
            Map(x => x.CONTRACT_VALUE_VAT);
            Map(x => x.PO_SO_NUMBER);
            Map(x => x.VAT);
            Map(x => x.CUSTOMER_CODE).Not.Update();
            Map(x => x.EXTEND_DATE);
            Map(x => x.FINISH_DATE);
            Map(x => x.NAME);
            Map(x => x.NOTES);
            Map(x => x.PARENT_CODE).Not.Update();
            Map(x => x.PROJECT_ID);
            Map(x => x.REPRESENT_A);
            Map(x => x.REPRESENT_B);
            Map(x => x.IS_SIGN_WITH_CUSTOMER).Not.Update();
            Map(x => x.START_DATE);
            Map(x => x.VENDOR_CODE).Not.Update();
            Map(x => x.PAYMENT_STATUS);
            Map(x => x.NGUOI_PHU_TRACH);
            Map(x => x.REFERENCE_FILE_ID).Not.Update();

            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Vendor).Columns("VENDOR_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Customer).Columns("CUSTOMER_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.ContractType).Columns("CONTRACT_TYPE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.PaymentStatus).Columns("PAYMENT_STATUS").Not.Insert().Not.Update().LazyLoad();
            References(x => x.ParentContract).Columns("PARENT_CODE").Not.Insert().Not.Update().LazyLoad();

            HasMany(x => x.Details).KeyColumn("CONTRACT_ID").Inverse().Cascade.All();
        }
    }
}
