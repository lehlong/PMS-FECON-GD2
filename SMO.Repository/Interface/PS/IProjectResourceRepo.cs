using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;

using System.Collections.Generic;

namespace SMO.Repository.Interface.PS
{
    public interface IProjectResourceRepo : IGenericRepository<T_PS_RESOURCE>
    {
        IList<T_AD_USER> SearchUser(T_AD_USER objFilter, int pageSize, int pageIndex, out int total);
    }
}
