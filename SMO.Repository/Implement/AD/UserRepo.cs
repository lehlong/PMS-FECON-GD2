using NHibernate.Criterion;
using NHibernate.Transform;
using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.AD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.AD
{
    public class UserRepo : GenericRepository<T_AD_USER>, IUserRepo
    {
        public UserRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_AD_USER> Search(T_AD_USER objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_AD_USER>();

            if (!string.IsNullOrWhiteSpace(objFilter.USER_NAME))
            {
                query = query.Where(x => x.USER_NAME.IsLike(string.Format("%{0}%", objFilter.USER_NAME)) || x.FULL_NAME.IsLike(string.Format("%{0}%", objFilter.USER_NAME)));
            }

            if (!string.IsNullOrWhiteSpace(objFilter.USER_TYPE))
            {
                query = query.Where(x => x.USER_TYPE == objFilter.USER_TYPE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.VENDOR_CODE))
            {
                query = query.Where(x => x.VENDOR_CODE == objFilter.VENDOR_CODE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.COMPANY_ID))
            {
                query = query.Where(x => x.COMPANY_ID == objFilter.COMPANY_ID);
            }

            query = query.Fetch(x => x.Organize).Eager;

            return base.Paging(query, pageSize, pageIndex, out total);
        }

        public override T_AD_USER Get(object id, dynamic param = null)
        {
            var query = NHibernateSession.QueryOver<T_AD_USER>();

#pragma warning disable CS0253 // Possible unintended reference comparison; to get a value comparison, cast the right hand side to type 'string'
            query = query.Where(x => x.USER_NAME == id );
#pragma warning restore CS0253 // Possible unintended reference comparison; to get a value comparison, cast the right hand side to type 'string'

            if (UtilsRepo.IsPropertyExist(param, "IsFetch_ListUserUserGroup"))
            {
                query = query.Fetch(x => x.ListUserUserGroup).Eager
                        .Fetch(x => x.ListUserUserGroup.First().UserGroup).Eager
                        .Fetch(x => x.ListUserUserGroup.First().UserGroup.ListUserGroupRole).Eager
                        .Fetch(x => x.ListUserUserGroup.First().UserGroup.ListUserGroupRole.First().Role).Eager;
            }

            if (UtilsRepo.IsPropertyExist(param, "IsFetch_ListUserRight"))
            {
                query = query.Fetch(x => x.ListUserRight).Eager;
            }

            if (UtilsRepo.IsPropertyExist(param, "IsFetch_ListUserRole"))
            {
                query = query.Fetch(x => x.ListUserRole).Eager
                    .Fetch(x => x.ListUserRole.First().Role).Eager;
            }

            if (UtilsRepo.IsPropertyExist(param, "IsFetch_Organize"))
            {
                query = query.Fetch(x => x.Organize).Eager;
            }

            query = query.TransformUsing(new DistinctRootEntityResultTransformer());
            return query.List().FirstOrDefault();
        }
    }
}
