using SMO.Core.Entities.MD;
using SMO.Repository.Common;
using SMO.Repository.Interface.MD;

using System.Collections.Generic;
using System.Linq;

namespace SMO.Repository.Implement.MD
{
    public class PaymentStatusRepo : GenericRepository<T_MD_PAYMENT_STATUS>, IPaymentStatusRepo
    {
        public PaymentStatusRepo(NHUnitOfWork unitOfWork) : base(unitOfWork.Session)
        {

        }

        public override IList<T_MD_PAYMENT_STATUS> Search(T_MD_PAYMENT_STATUS objFilter, int pageSize, int pageIndex, out int total)
        {
            var query = Queryable();

            if (!string.IsNullOrWhiteSpace(objFilter.NAME))
            {
                query = query.Where(x => x.NAME.ToLower().Contains(objFilter.NAME.ToLower()) || x.CODE.ToLower().Contains(objFilter.NAME.ToLower()));
            }
            return base.Paging(query, pageSize, pageIndex, out total).ToList();
        }
    }
}
