using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SMO.Core.Entities
{
    public partial class T_CF_TEMPLATE_NOTIFY : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string CONG_VIEC_XU_LY_SUBJECT { get; set; }
        public virtual string CONG_VIEC_XU_LY_BODY { get; set; }
        public virtual string CONG_VIEC_HOAN_THANH_SUBJECT { get; set; }
        public virtual string CONG_VIEC_HOAN_THANH_BODY { get; set; }
        public virtual string PHE_DUYET_CHINH_SUA_SUBJECT { get; set; }
        public virtual string PHE_DUYET_CHINH_SUA_BODY { get; set; }
        public virtual string NHAN_SU_THAM_GIA_SUBJECT { get; set; }
        public virtual string NHAN_SU_THAM_GIA_BODY { get; set; }
    }
}
