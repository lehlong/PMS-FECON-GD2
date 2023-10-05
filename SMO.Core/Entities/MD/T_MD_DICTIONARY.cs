using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Core.Entities
{
    public class T_MD_DICTIONARY : BaseEntity
    {
        public virtual string PKID { get; set; }
        public virtual string FK_DOMAIN { get; set; }
        public virtual string CODE { get; set; }
        public virtual string LANG { get; set; }
        public virtual string C_VALUE { get; set; }
        public virtual int C_ORDER { get; set; }
        public virtual bool C_DEFAULT { get; set; }
#pragma warning disable CS0114 // 'T_MD_DICTIONARY.ACTIVE' hides inherited member 'BaseEntity.ACTIVE'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
        public virtual bool ACTIVE { get; set; }
#pragma warning restore CS0114 // 'T_MD_DICTIONARY.ACTIVE' hides inherited member 'BaseEntity.ACTIVE'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword.
    }
}
