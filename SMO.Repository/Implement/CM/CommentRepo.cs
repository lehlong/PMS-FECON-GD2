using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.CM;
using System.Collections.Generic;
using System.Linq;
using System;
using NHibernate.Linq;

namespace SMO.Repository.Implement.CM
{
    public class CommentRepo : GenericRepository<T_CM_COMMENT>, ICommentRepo
    {
        public CommentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }
        public List<T_CM_COMMENT> GetCommentsOfDocument(string referenceId)
        {
            var query = Queryable();
            query = query.Where(x => x.REFRENCE_ID == referenceId).OrderByDescending(x => x.CREATE_DATE);
            query = query.Fetch(x => x.USER_CREATE);
            return query.ToList();
        }
    }
}
