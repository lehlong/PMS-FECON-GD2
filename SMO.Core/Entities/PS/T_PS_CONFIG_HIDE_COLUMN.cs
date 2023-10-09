using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Core.Entities.PS
{
    public class T_PS_CONFIG_HIDE_COLUMN : BaseEntity
    {
        public virtual Guid ID { get; set; }
        public virtual string USER_NAME { get; set; }
        public virtual string TYPE_DISPLAY { get; set; }
        public virtual string DETAILS { get; set; }
        public virtual Guid PROJECT_ID { get; set; }

    }
}
