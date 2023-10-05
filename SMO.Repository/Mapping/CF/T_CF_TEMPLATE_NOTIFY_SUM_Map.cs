using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_TEMPLATE_NOTIFY_SUM_Map : BaseMapping<T_CF_TEMPLATE_NOTIFY_SUM>
    {
        public T_CF_TEMPLATE_NOTIFY_SUM_Map()
        {
            Table("T_CF_TEMPLATE_NOTIFY_SUM");
            Id(x => x.PO_STATUS);
            Map(x => x.SMS_TEMPLATE);
            Map(x => x.SUBJECT_TEMPLATE);
            Map(x => x.EMAIL_TEMPLATE);
        }
    }
}
