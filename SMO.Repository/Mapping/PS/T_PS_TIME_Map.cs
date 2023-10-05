using SMO.Core.Entities.PS;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Mapping.PS
{
    public class T_PS_TIME_Map : BaseMapping<T_PS_TIME>
    {
        public T_PS_TIME_Map()
        {
            Id(x => x.ID);
            Map(x => x.PROJECT_ID).Not.Nullable();
            Map(x => x.START_DATE).Not.Nullable();
            Map(x => x.FINISH_DATE).Not.Nullable();
            Map(x => x.TIME_TYPE).Not.Nullable();
            Map(x => x.C_ORDER).Not.Nullable();
            Map(x => x.MONTH);
            Map(x => x.YEAR).Not.Nullable();

            References(x => x.Project).Columns("PROJECT_ID").Not.Insert().Not.Update().LazyLoad();
            References(x => x.TimeType).Columns("TIME_TYPE").Not.Insert().Not.Update().LazyLoad();

        }
    }
}
