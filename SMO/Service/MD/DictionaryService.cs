using SMO.Core.Entities;
using SMO.Repository.Implement.MD;
using System;

namespace SMO.Service.MD
{
    public class DictionaryService : GenericService<T_MD_DICTIONARY, DictionaryRepo>
    {
        public DictionaryService() : base()
        {

        }

        public override void Create()
        {
            try
            {
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE && x.LANG == this.ObjDetail.LANG && x.FK_DOMAIN == this.ObjDetail.FK_DOMAIN))
                {
                    this.ObjDetail.ACTIVE = true;
                    this.ObjDetail.PKID = Guid.NewGuid().ToString();
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
