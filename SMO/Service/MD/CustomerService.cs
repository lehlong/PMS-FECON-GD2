using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using System;
using System.Linq;

namespace SMO.Service.MD
{
    public class CustomerService : GenericService<T_MD_CUSTOMER, CustomerRepo>
    {
        public Guid ProjectId { get; set; }
        public bool IsAccept { get; set; }
        public CustomerService() : base()
        {

        }
        public void SearchProjectVolume()
        {
            var contracts = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>()
                .Queryable()
                .Where(x => x.PROJECT_ID == ProjectId && x.IS_SIGN_WITH_CUSTOMER == true)
                .ToList();
            ObjList = contracts.Select(x => x.Customer).Distinct().ToList();
        }
        public override void Create()
        {
            try
            {
                this.ObjDetail.ACTIVE = true;
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE))
                {
                    base.Create();
                }
                else
                {
                    this.State = false;
                    this.MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
