using System;

namespace SMO.Core.Entities
{
    public partial class T_AD_MENU : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string DESCRIPTION { get; set; }
        public virtual string PARENT { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual string FK_RIGHT { get; set; }
        public virtual string LINK { get; set; }
        public virtual string ICON { get; set; }
        //Thuộc tính thêm để lấy dữ liệu từ bảng Language
        public virtual string LANGUAGE_VALUE { get; set; }
    }
}
