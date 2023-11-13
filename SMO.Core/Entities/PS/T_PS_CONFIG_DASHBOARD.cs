using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONFIG_DASHBOARD : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string C_ORDER { get; set; }
        public virtual Guid PROJECT_ID { get; set; }

    }
}
