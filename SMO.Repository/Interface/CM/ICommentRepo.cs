using SMO.Core.Entities;
using SMO.Repository.Common;
using System.Collections.Generic;

namespace SMO.Repository.Interface.CM
{
    public interface ICommentRepo : IGenericRepository<T_CM_COMMENT>
    {
        List<T_CM_COMMENT> GetCommentsOfDocument(string referenceId);
    }
}
