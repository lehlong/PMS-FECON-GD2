using SMO.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.CF;
using SMO.Core.Entities;

namespace SMO.Service.CF
{
    public class ConfigService : BaseService
    {
        public IUnitOfWork UnitOfWork { get; set; }
        public string CompanyCode { get; set; }
        public string ModulType { get; set; }
        public string ModulTypeOfDetail { get; set; }
        public string ConfigType { get; set; }
        public T_AD_ORGANIZE ObjOrganize { get; set; }
        public T_CF_MODUL ObjConfigModul { get; set; }
        public T_CF_DCCH ObjConfigDCCH { get; set; }
        public List<T_CF_MODUL> ObjListConfigModul { get; set; }
        public ConfigService()
        {
            UnitOfWork = new NHUnitOfWork();
            ObjConfigModul = new T_CF_MODUL();
            ObjConfigDCCH = new T_CF_DCCH();
        }

        public void GetInfoOrganzie()
        {
            this.ObjOrganize = this.UnitOfWork.Repository<OrganizeRepo>().Queryable().SingleOrDefault(x => x.COMPANY_CODE == this.CompanyCode);
            this.ObjListConfigModul = this.UnitOfWork.Repository<ConfigMODULRepo>().Queryable().Where(x => x.COMPANY_CODE == this.CompanyCode).ToList();
        }

        public List<NodeConfig> BuildTree()
        {
            var lstNode = new List<NodeConfig>();
            var lstCompany = this.UnitOfWork.Repository<OrganizeRepo>().Queryable().Where(x => x.TYPE == "CP").ToList();
            lstNode.Add(new NodeConfig() { id = "0", name = "Thông tin cấu hình" });
            foreach (var company in lstCompany.OrderBy(x => x.CREATE_DATE))
            {
                var nodeCompany = new NodeConfig()
                {
                    id = company.PKID,
                    pId = "0",
                    name = company.NAME,
                    companyCode = company.COMPANY_CODE
                };
                lstNode.Add(nodeCompany);

                // Danh sách modul
                var lstModul = this.UnitOfWork.Repository<ConfigMODULRepo>().Queryable().Where(x => x.COMPANY_CODE == company.COMPANY_CODE).ToList();
                foreach (var modul in lstModul.OrderBy(x => x.CREATE_DATE))
                {
                    var nodeModul = new NodeConfig()
                    {
                        id = modul.PKID,
                        pId = company.PKID,
                        name = SMO.ModulType.GetText(modul.MODUL_TYPE),
                        companyCode = company.COMPANY_CODE,
                        modulType = modul.MODUL_TYPE
                    };
                    lstNode.Add(nodeModul);

                    if (modul.MODUL_TYPE == SMO.ModulType.DCCH)
                    {
                        var lstDetail = this.UnitOfWork.GetSession().QueryOver<T_CF_DCCH>().Where(x => x.COMPANY_CODE == company.COMPANY_CODE).Fetch(x => x.SaleOffice).Eager.List();
                        foreach (var detail in lstDetail.OrderBy(x => x.CREATE_DATE))
                        {
                            var nodeDetail = new NodeConfig()
                            {
                                id = detail.PKID,
                                pId = modul.PKID,
                                name = detail.SaleOffice.TEXT,
                                companyCode = company.COMPANY_CODE,
                                modulType = modul.MODUL_TYPE
                            };
                            lstNode.Add(nodeDetail);
                        }
                    }
                }
            }
            return lstNode;
        }

        public void Create()
        {
            try
            {
                if (this.ConfigType == "MODUL")
                {
                    if (this.UnitOfWork.Repository<ConfigMODULRepo>().CheckExist(x => x.COMPANY_CODE == this.CompanyCode && x.MODUL_TYPE == this.ModulType))
                    {
                        this.State = false;
                        this.ErrorMessage = "Cấu hình cho modul '" + SMO.ModulType.GetText(this.ModulType) + "' đã tồn tại!";
                        return;
                    }
                    this.UnitOfWork.BeginTransaction();
                    this.UnitOfWork.Repository<ConfigMODULRepo>().Create(new T_CF_MODUL() {
                        PKID = Guid.NewGuid().ToString(),
                        COMPANY_CODE = this.CompanyCode,
                        MODUL_TYPE = this.ModulType,
                        CREATE_BY = ProfileUtilities.User.USER_NAME
                    });
                    this.UnitOfWork.Commit();
                }

                if (this.ConfigType == "DETAIL")
                {
                    if (this.ModulTypeOfDetail == SMO.ModulType.DCCH)
                    {
                        if (this.UnitOfWork.Repository<ConfigDCCHRepo>().CheckExist(x => x.COMPANY_CODE == this.CompanyCode && x.SALEOFFICE_CODE == this.ObjConfigDCCH.SALEOFFICE_CODE))
                        {
                            this.State = false;
                            this.ErrorMessage = "Cấu hình cho cửa hàng '" + SMO.ModulType.GetText(this.ModulType) + "' đã tồn tại!";
                            return;
                        }
                        this.UnitOfWork.BeginTransaction();
                        this.ObjConfigDCCH.COMPANY_CODE = this.CompanyCode;
                        this.ObjConfigDCCH.PKID = Guid.NewGuid().ToString();
                        this.ObjConfigDCCH.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        this.UnitOfWork.Repository<ConfigDCCHRepo>().Create(this.ObjConfigDCCH);
                        this.UnitOfWork.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }
    }
}