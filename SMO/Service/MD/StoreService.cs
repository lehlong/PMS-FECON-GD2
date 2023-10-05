using NHibernate;
using SharpSapRfc;
using SharpSapRfc.Plain;
using SMO.Core.Entities;
using SMO.Repository.Implement.CF;
using SMO.Repository.Implement.MD;
using SMO.SAPINT;
using SMO.SAPINT.Function;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SMO.Service.MD
{
    public class StoreService : GenericService<T_MD_STORE, StoreRepo>
    {
        public T_MD_MATERIAL ObjMaterial { get; set; }
        public List<T_MD_MATERIAL> ObjListMaterial { get; set; }
        public StoreService() : base()
        {
            ObjMaterial = new T_MD_MATERIAL();
            ObjListMaterial = new List<T_MD_MATERIAL>();
        }

        public void SearchMaterialForAdd()
        {
            this.Get(this.ObjDetail.CODE);
            var lstMaterialOfStore = this.ObjDetail.ListStoreMaterial.Select(x => x.MATERIAL_CODE).ToList();
            var query = UnitOfWork.Repository<MaterialRepo>().Queryable();
            query = query.Where(x => !lstMaterialOfStore.Contains(x.CODE));
            query = query.Where(x => x.TYPE == "ZXD");
            if (!string.IsNullOrWhiteSpace(ObjMaterial.CODE))
            {
                query = query.Where(x => x.CODE.ToLower().Contains(ObjMaterial.CODE.ToLower()) || x.TEXT.ToLower().Contains(ObjMaterial.CODE.ToLower()));
            }
            this.ObjListMaterial = query.ToList();
        }

        public void AddMaterialToGroup(string lstMaterial, string storeCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var material in lstMaterial.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = new T_CF_STORE_MATERIAL()
                    {
                        STORE_CODE = storeCode,
                        MATERIAL_CODE = material
                    };

                    if (ProfileUtilities.User != null)
                    {
                        item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        item.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                    }
                    UnitOfWork.Repository<StoreMaterialRepo>().Create(item);
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public void DeleteMaterialOfStore(string lstMaterial, string storeCode)
        {
            try
            {
                UnitOfWork.BeginTransaction();

                foreach (var material in lstMaterial.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                {
                    var item = UnitOfWork.Repository<StoreMaterialRepo>().Queryable().FirstOrDefault(x => x.STORE_CODE == storeCode && x.MATERIAL_CODE == material);
                    if (item != null)
                    {
                        UnitOfWork.Repository<StoreMaterialRepo>().Delete(item);
                    }
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}
