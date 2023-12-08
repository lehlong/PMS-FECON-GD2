using SMO.Core.Entities.PS;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_CONTRACT_REQUEST_PAYMENT_Map : BaseMapping<T_PS_CONTRACT_REQUEST_PAYMENT>
    {
        public T_PS_CONTRACT_REQUEST_PAYMENT_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned(); ;
            Map(x => x.CONTRACT_ID);
            Map(x => x.AMOUNT);
            Map(x => x.AMOUNT_ADVANCE);
            Map(x => x.PAYMENT_DATE);
            Map(x => x.BILL_NUMBER);
            Map(x => x.CONTENTS);
            Map(x => x.EXPLAIN);
            Map(x => x.STATUS);
            Map(x => x.INVOICE_VALUE);
            Map(x => x.REFERENCE_FILE_ID).Not.Update();
            Map(x => x.WORKFLOW_ID);
            Map(x => x.REQUEST_TYPE_CODE);
            Map(x => x.EXCHANGE_RATE);
            Map(x => x.CURRENCY_CODE);
            References(x => x.PaymentStatus).Columns("STATUS").Not.Insert().Not.Update().LazyLoad();
            References(x => x.RequestType).Columns("REQUEST_TYPE_CODE").Not.Insert().Not.Update().LazyLoad();
            References(x => x.Workflow).Columns("WORKFLOW_ID").Not.Insert().Not.Update().LazyLoad();
        }
    }
}
