
using SMO.Core.Common;

using System;
using System.Collections.Generic;

namespace SMO.Core.Entities.PS
{
    public class T_PS_VOLUME_ACCEPT : BaseProjectVolumeEntity
    {
        public virtual DateTime? FILTER_TO_DATE { get; set; }
        public virtual DateTime? FILTER_FROM_DATE { get; set; }
        public T_PS_VOLUME_ACCEPT()
        {
            Details = new List<T_PS_VOLUME_ACCEPT_DETAIL>();
        }
        public virtual IList<T_PS_VOLUME_ACCEPT_DETAIL> Details { get; set; }
    }
}
