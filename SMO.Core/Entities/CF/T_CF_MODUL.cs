using SharpSapRfc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMO.Core.Entities
{
    public partial class T_CF_MODUL : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string COMPANY_CODE { get; set; }
        public virtual string MODUL_TYPE { get; set; }
    }
}
