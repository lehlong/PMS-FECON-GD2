using NHibernate.Linq;
using NHibernate;
using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Transform;
using NHibernate.Criterion;

namespace SMO.Repository.Implement.MD
{
    public class StoreRepo : GenericRepository<T_MD_STORE>, IStoreRepo
    {
        public StoreRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_STORE> Search(T_MD_STORE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_MD_STORE>();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.IsLike(string.Format("%{0}%", objFilter.CODE.ToLower())) || x.TEXT.IsLike(string.Format("%{0}%", objFilter.CODE.ToLower())));
            }
            
            query = query.Fetch(x => x.ListStoreMaterial).Eager
                .Fetch(x => x.ListStoreMaterial.First().Material).Eager
                .TransformUsing(new DistinctRootEntityResultTransformer());
            total = 0;
            return query.Future().ToList();
        }
    }
}
