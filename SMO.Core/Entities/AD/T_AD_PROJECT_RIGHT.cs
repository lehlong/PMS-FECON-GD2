using System;
using System.Collections.Generic;

namespace SMO.Core.Entities
{
    [Serializable]
    public partial class T_AD_PROJECT_RIGHT : BaseEntity
    {
        public virtual string CODE { get; set; }
        public virtual string NAME { get; set; }
        public virtual string PARENT { get; set; }
        public virtual int C_ORDER { get; set; }
    }
}
