using NHibernate.Criterion;
using NHibernate.Linq;
using NHibernate.Transform;
using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class VehicleRepo : GenericRepository<T_MD_VEHICLE>, IVehicleRepo
    {
        public VehicleRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_VEHICLE> Search(T_MD_VEHICLE objFilter, int pageSize, int pageIndex, out int total)
        {
            int startIndex = pageSize * (pageIndex - 1);
            var query = NHibernateSession.QueryOver<T_MD_VEHICLE>();
            var subQuery = QueryOver.Of<T_MD_VEHICLE>();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.IsLike($"%{objFilter.CODE}%"));
                subQuery = subQuery.Where(x => x.CODE.IsLike($"%{objFilter.CODE}%"));
            }

            var queryListDetail = NHibernateSession.QueryOver<T_MD_VEHICLE_COMPARTMENT>()
                .WithSubquery.WhereProperty(x => x.VEHICLE_CODE).In(
                    subQuery.Select(Projections.Property<T_MD_VEHICLE>(p => p.CODE))
                    .OrderBy(p => p.ACTIVE).Desc
                    .Take(pageSize).Skip(startIndex))
                   .Future();

            query = query.OrderBy(x => x.ACTIVE).Desc;
            //query = query.Fetch(x => x.ListCompartment).Eager;
            var result = base.Paging(query, pageSize, pageIndex, out total).ToList();
            var lstDetail = queryListDetail.ToList();
            foreach (var item in result)
            {
                item.ListCompartment = lstDetail.Where(x => x.VEHICLE_CODE == item.CODE).ToList();
            }
            return result;
        }
    }
}
