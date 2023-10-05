using SMO.Core.Entities.PS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_GEN_CODE_HISTORY_Map : BaseMapping<T_PS_GEN_CODE_HISTORY>
    {
        public T_PS_GEN_CODE_HISTORY_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_ID);
            Map(x => x.GEN_CODE);
            Map(x => x.TYPE);
        }
    }
}
