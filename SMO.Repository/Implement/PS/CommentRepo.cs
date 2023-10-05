using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Interface.PS;

using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.PS
{
    public class CommentRepo : GenericRepository<T_PS_COMMENT>, ICommentRepo
    {
        public CommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {
        }
    }
}
