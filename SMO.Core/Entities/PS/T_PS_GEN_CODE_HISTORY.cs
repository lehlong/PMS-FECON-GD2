using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Core.Entities.PS
{
    public class T_PS_GEN_CODE_HISTORY : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual Guid PROJECT_ID { get; set; }
        public virtual string GEN_CODE { get; set; }
        public virtual string TYPE { get; set; }
    }
}
