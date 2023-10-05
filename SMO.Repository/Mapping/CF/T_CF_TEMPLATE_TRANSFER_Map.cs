using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_TEMPLATE_TRANSFER_Map : BaseMapping<T_CF_TEMPLATE_TRANSFER>
    {
        public T_CF_TEMPLATE_TRANSFER_Map()
        {
            Table("T_CF_TEMPLATE_TRANSFER");
            Id(x => x.PKID);
            Map(x => x.SMS_DES_TEMPLATE);
            Map(x => x.SUBJECT_DES_TEMPLATE);
            Map(x => x.EMAIL_DES_TEMPLATE);
            Map(x => x.SMS_SOURCE_TEMPLATE);
            Map(x => x.SUBJECT_SOURCE_TEMPLATE);
            Map(x => x.EMAIL_SOURCE_TEMPLATE);
        }
    }
}
