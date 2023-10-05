using SMO.Core.Common;
using System;
using System.Collections.Generic;

namespace SMO.Core.Entities.PS
{
    public class T_PS_VOLUME_WORK : BaseProjectVolumeEntity
    {
        public virtual DateTime? FILTER_TO_DATE { get; set; }
        public virtual DateTime? FILTER_FROM_DATE { get; set; }

        public T_PS_VOLUME_WORK()
        {
            Details = new List<T_PS_VOLUME_WORK_DETAIL>();
        }


        public virtual IList<T_PS_VOLUME_WORK_DETAIL> Details { get; set; }

    }
}
