using SMO.Core.Entities;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class VehicleCompartmentRepo : GenericRepository<T_MD_VEHICLE_COMPARTMENT>, IVehicleCompartmentRepo
    {
        public VehicleCompartmentRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_VEHICLE_COMPARTMENT> Search(T_MD_VEHICLE_COMPARTMENT objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(objFilter.CODE.ToLower()));
            }

            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
