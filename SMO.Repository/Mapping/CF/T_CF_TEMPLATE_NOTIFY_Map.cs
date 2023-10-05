using NHibernate.Type;
using SMO.Core.Entities;

namespace SMO.Repository.Mapping.CF
{
    public class T_CF_TEMPLATE_NOTIFY_Map : BaseMapping<T_CF_TEMPLATE_NOTIFY>
    {
        public T_CF_TEMPLATE_NOTIFY_Map()
        {
            Table("T_CF_TEMPLATE_NOTIFY");
            Id(x => x.PKID);
            Map(x => x.CONG_VIEC_HOAN_THANH_SUBJECT);
            Map(x => x.CONG_VIEC_HOAN_THANH_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.CONG_VIEC_XU_LY_SUBJECT);
            Map(x => x.CONG_VIEC_XU_LY_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.PHE_DUYET_CHINH_SUA_SUBJECT);
            Map(x => x.PHE_DUYET_CHINH_SUA_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
            Map(x => x.NHAN_SU_THAM_GIA_SUBJECT);
            Map(x => x.NHAN_SU_THAM_GIA_BODY).CustomType("StringClob").CustomSqlType("nvarchar(max)");
        }
    }
}
