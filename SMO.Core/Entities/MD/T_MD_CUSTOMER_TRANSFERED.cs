using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Core.Entities
{
    public class T_MD_CUSTOMER_OLD_TRANSFERED : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string CUSTOMER_CODE_SOURCE { get; set; }
        public virtual string COMPANY_CODE_SOURCE { get; set; }
        public virtual string CUSTOMER_CODE_DES { get; set; }
        public virtual string COMPANY_CODE_DES { get; set; }
        public virtual T_MD_CUSTOMER_OLD CustomerSource { get; set; }
        public virtual T_MD_CUSTOMER_OLD CustomerDes { get; set; }
    }
}
