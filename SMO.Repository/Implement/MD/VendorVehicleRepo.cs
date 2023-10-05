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
    public class VendorVehicleRepo : GenericRepository<T_MD_VENDOR_VEHICLE>, IVendorVehicleRepo
    {
        public VendorVehicleRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_VENDOR_VEHICLE> Search(T_MD_VENDOR_VEHICLE objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = NHibernateSession.QueryOver<T_MD_VENDOR_VEHICLE>();

            if (!string.IsNullOrWhiteSpace(objFilter.VENDOR_CODE))
            {
                query = query.Where(x => x.VENDOR_CODE == objFilter.VENDOR_CODE);
            }

            if (!string.IsNullOrWhiteSpace(objFilter.VEHICLE_CODE))
            {
                query = query.Where(x => x.VEHICLE_CODE.IsLike(string.Format("%{0}%", objFilter.VEHICLE_CODE)));
            }

            query = query.OrderBy(x => x.CREATE_DATE).Desc
                .Fetch(x => x.Vehicle).Eager
                .Fetch(x => x.Vehicle.ListCompartment).Eager
                .TransformUsing(Transformers.DistinctRootEntity);
            total = 0;
            return query.List();
        }
    }
}
