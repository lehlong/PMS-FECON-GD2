using FluentNHibernate.Mapping;
using SMO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMO.Repository.Mapping
{
    public class BaseMapping<T> : ClassMap<T> where T : BaseEntity
    {
        public BaseMapping()
        {
            Map(m => m.CREATE_BY).Nullable().Not.Update();
            References(x => x.USER_CREATE).Column("CREATE_BY").Not.Insert().Not.Update();
            Map(m => m.CREATE_DATE).Nullable().Not.Update();
            Map(m => m.UPDATE_BY).Nullable();
            References(x => x.USER_UPDATE).Column("UPDATE_BY").Not.Insert().Not.Update();
            Map(m => m.UPDATE_DATE).Nullable();
        }
    }
}
