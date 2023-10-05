using SMO.Core.Entities;
using SMO.Core.Entities.MD;
using SMO.Repository.Implement.MD;
using System;
using System.Linq;

namespace SMO.Service.MD
{
    public class VendorService : GenericService<T_MD_VENDOR, VendorRepo>
    {
        public Guid ProjectId { get; set; }
        public bool IsAccept { get; set; }
        public VendorService() : base()
        {

        }
        public void SearchProjectVolume()
        {
            var contracts = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>()
                .Queryable()
                .Where(x => x.PROJECT_ID == ProjectId && x.IS_SIGN_WITH_CUSTOMER == false)
                .ToList();
            ObjList = contracts.Where(x => x.Vendor != null).Select(x => x.Vendor).Distinct().ToList();
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
