using SMO.Core.Entities.PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_CONFIG_DASHBOARD_Map : BaseMapping<T_PS_CONFIG_DASHBOARD>
    {
        public T_PS_CONFIG_DASHBOARD_Map()
        {
            Id(x => x.ID).GeneratedBy.Assigned();
            Map(x => x.USER_NAME);
            Map(x => x.C_ORDER);
            Map(x => x.PROJECT_ID);
        }
    }
}
