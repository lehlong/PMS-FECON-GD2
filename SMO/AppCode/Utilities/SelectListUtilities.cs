using SMO.Core.Entities;
using SMO.Core.Entities.PS;
using SMO.Repository.Common;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;
using SMO.Repository.Implement.PS;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMO
{
    public class SelectListUtilities
    {
        public class Data
        {
            public string Value { get; set; }
            public string Text { get; set; }
        }

        public class DataVersion
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        /// <summary>
        /// Lấy ra danh sách Domain để chọn dropdownlist 
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllDomain(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstDomain = UnitOfWork.Repository<DomainRepo>().GetAll();
            foreach (var obj in lstDomain)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE + " - " + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        public static SelectList GetVersionStructCost(Guid projectId)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<DataVersion>();
            var lstVersion = UnitOfWork.Repository<ProjectStructVersionRepo>().Queryable().Where(x => x.PROJECT_ID == projectId).ToList();            
            if (lstVersion.Count() != 0)
            {
                lstData.Add(new DataVersion { Value = 0, Text = $"Version {lstVersion.Max(x => x.VERSION) + 1}" });
                foreach (var ver in lstVersion.OrderByDescending(x => x.VERSION).Select(x => x.VERSION).Distinct().ToList())
                {
                    lstData.Add(new DataVersion { Value = ver, Text = $"Version {ver}" });
                }
            }
            else
            {
                lstData.Add(new DataVersion { Value = 0, Text = $"Version 1" });
            }
            return new SelectList(lstData, "Value", "Text", new DataVersion {});
        }
        public static SelectList GetTypeResource ()
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            
            var lstDomain = UnitOfWork.Repository<DomainRepo>().GetAll();
            
            lstData.Add(new Data { Value = "", Text = "-"});
            lstData.Add(new Data { Value = "FECON", Text ="Nhân sự FECON" });
            lstData.Add(new Data { Value = "OTHER", Text = "Các bên liên quan"});


            return new SelectList(lstData, "Value", "Text", new Data {});
        }

        /// <summary>
        /// Lấy ra danh sách chọn cho dropdownlist
        /// </summary>
        /// <param name="strDomainCode">Mã domain</param>
        /// <param name="isShowValue">Có hiển thị cả mã dictionary trên dropdownlist không. Ví dụ : vn - Việt Nam</param>
        /// <param name="isAddBlank">Có thêm giá trị blank vào đầu tiên trong list chọn của dropdownlist hay không</param>
        /// <param name="selected">Giá trị được chọn mặc định khi hiển thị dropdownlist</param>
        /// <param name="strLang">Ngôn ngữ - để lấy ra giá trị của dictionary theo ngôn ngữ</param>
        /// <param name="isShowDefault"></param>
        /// <returns></returns>
        public static SelectList GetDictionary(string strDomainCode, bool isShowValue = true, bool isAddBlank = true, string selected = "", string strLang = "vi", bool isShowDefault = true)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            var strValue = string.Empty;
            var listDic = UnitOfWork.Repository<DictionaryRepo>().Queryable().Where(x => x.FK_DOMAIN == strDomainCode && x.LANG == strLang).OrderBy(x => x.C_ORDER).ToList() ;
            if (isAddBlank)
            {
                lstData.Add(new Data { Value = string.Empty, Text = " - " });
            }
            foreach (var dic in listDic)
            {
                if (string.IsNullOrWhiteSpace(selected) && dic.C_DEFAULT == true && isShowDefault == true)
                {
                    selected = dic.CODE;
                }
                strValue = dic.CODE + " - " + dic.C_VALUE;
                if (!isShowValue)
                {
                    strValue = dic.C_VALUE;
                }
                lstData.Add(new Data { Value = dic.CODE, Text = strValue });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetNoiNhanLenh()
        {
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = NoiNhanLenh.VanPhong, Text = NoiNhanLenh.GetText(NoiNhanLenh.VanPhong) });
            lstData.Add(new Data() { Value = NoiNhanLenh.NhaBe, Text = NoiNhanLenh.GetText(NoiNhanLenh.NhaBe) });
            return new SelectList(lstData, "Value", "Text");
        }

        public static SelectList GetPhuongThucThanhToan()
        {
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = PaymentMethod.Befor, Text = PaymentMethod.GetText(PaymentMethod.Befor) });
            lstData.Add(new Data() { Value = PaymentMethod.After, Text = PaymentMethod.GetText(PaymentMethod.After) });
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllModulType()
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = ModulType.DCCH, Text = ModulType.GetText(ModulType.DCCH) });
            lstData.Add(new Data() { Value = ModulType.DCNB, Text = ModulType.GetText(ModulType.DCNB) });
            lstData.Add(new Data() { Value = ModulType.MHGL, Text = ModulType.GetText(ModulType.MHGL) });
            lstData.Add(new Data() { Value = ModulType.XBND, Text = ModulType.GetText(ModulType.XBND) });
            lstData.Add(new Data() { Value = ModulType.XBTX, Text = ModulType.GetText(ModulType.XBTX) });
            lstData.Add(new Data() { Value = ModulType.XTHG, Text = ModulType.GetText(ModulType.XTHG) });
            
            return new SelectList(lstData, "Value", "Text");
        }

        public static SelectList GetAllPoStatus()
        {
            var lstData = new List<Data>();
            foreach (var item in PO_Status.ListStatus)
            {
                lstData.Add(new Data() { Value = item, Text = PO_Status.GetStatusTextSaleManager(item) });
            }
            return new SelectList(lstData, "Value", "Text");
        }
        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetModulTypeOfCompany(List<T_CF_MODUL> lstModul)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            foreach (var item in lstModul)
            {
                lstData.Add(new Data() { Value = item.MODUL_TYPE, Text = ModulType.GetText(item.MODUL_TYPE) });
            }
            return new SelectList(lstData, "Value", "Text");
        }


        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllForm(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstForm = UnitOfWork.Repository<FormRepo>().GetAll();
            foreach (var obj in lstForm)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE + " - " + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllRight(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<RightRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.C_ORDER))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE + " - " + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetAllOrganize(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<OrganizeRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.C_ORDER))
            {
                lstData.Add(new Data { Value = obj.PKID, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetCompany(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<OrganizeRepo>().GetAll();
            foreach (var obj in lstAll.Where(x => x.TYPE == "CP").OrderBy(x => x.C_ORDER))
            {
                lstData.Add(new Data { Value = obj.PKID, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetVehicle(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<VehicleRepo>().Queryable().Where(x => x.ACTIVE).ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetVendor(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<VendorOldRepo>().Queryable().Where(x => x.ACTIVE).ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetVehicle(List<T_MD_VEHICLE> lstVehicle)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            foreach (var obj in lstVehicle)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetSaleOffice(bool isAddBlank = true)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<SaleOfficeRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetSaleOffice(string companyCode)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            var lstAll = UnitOfWork.Repository<SaleOfficeRepo>().Queryable().Where(x => x.COMPANY_CODE == companyCode).ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetSaleOffice(List<T_MD_SALEOFFICE> lstSaleOffice)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            foreach (var obj in lstSaleOffice)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        public static SelectList GetCustomer()
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            var lstActive = UnitOfWork.Repository<CustomerOldRepo>().GetListActive();
            foreach (var obj in lstActive)
            {
                lstData.Add(new Data { Value = obj.CUSTOMER_CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        public static SelectList GetMaterial()
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            var lstActive = UnitOfWork.Repository<MaterialRepo>().GetListActive();
            foreach (var obj in lstActive)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetCustomer(string companyCode = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            var query = UnitOfWork.Repository<CustomerOldRepo>().Queryable();
            query = query.Where(x => x.ACTIVE == true);
            if (!string.IsNullOrEmpty(companyCode))
            {
                query = query.Where(x => x.COMPANY_CODE == companyCode);
            }
            var lstAll = query.ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CUSTOMER_CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetUnit(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<UnitOldRepo>().GetListActive();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text", new Data { Value = selected });
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetUnit(List<T_MD_UNIT_OLD> lstUnit)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            foreach (var obj in lstUnit)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetCurrency()
        {
            var lstData = new List<Data>();
            lstData.Add(new Data { Value = "", Text = "" });
            lstData.Add(new Data { Value = "VND", Text = "VND" });
            lstData.Add(new Data { Value = "USD", Text = "USD" });
            lstData.Add(new Data { Value = "EUR", Text = "EUR" });
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetCustomer(List<T_MD_CUSTOMER_OLD> lstCustomer)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            foreach (var obj in lstCustomer)
            {
                lstData.Add(new Data { Value = obj.CUSTOMER_CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text");
        }

        /// <summary>
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static SelectList GetContract(List<T_MD_CONTRACT> lstContract)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();
            lstData.Add(new Data() { Value = "", Text = " - " });
            foreach (var obj in lstContract)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE });
            }
            return new SelectList(lstData, "Value", "Text");
        }
        public static SelectList GetContractByProject(Guid projectId, ProjectEnum projectType)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var contracts = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>()
                .Queryable()
                .Where(x => x.PROJECT_ID == projectId)
                .Where(x => x.IS_SIGN_WITH_CUSTOMER == (projectType == ProjectEnum.BOQ))
                .ToList();

            var lstData = new List<Data>
            {
                //new Data() { Value = Guid.Empty.ToString(), Text = " - " }
            };
            foreach (var obj in contracts)
            {
                lstData.Add(new Data { Value = obj.ID.ToString(), Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text");
        }
        public static SelectList GetTimePeriods(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectTimePeriodRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.TEXT });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetProjectLevel(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectLevelRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetPurchaseType(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<PurchaseTypeRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetRequestType(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<RequestTypeRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetActionWorkflow(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ActionWorkflowRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetProjectRole(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectRoleRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.ID, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetProjectType(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectTypeRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetAuthority(bool isAddBlank = true, string selected = "")
        {
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }

            lstData.Add(new Data { Value = "true", Text = "Có" });
            lstData.Add(new Data { Value = "false", Text = "Không" });
            
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetListUserFecon(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<UserRepo>().Queryable().Where(x => x.USER_TYPE == UserType.Fecon).ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.USER_NAME, Text = obj.USER_NAME + "-" + obj.FULL_NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetProjects(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = Guid.Empty.ToString(), Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectRepo>().GetAllOrdered("NAME");
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.ID.ToString(), Text = $"[{obj.CODE}]-{obj.NAME}" });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }
        
        public static SelectList GetContractTypes(bool isCustomer, bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ContractTypeRepo>().GetAllOrdered("NAME");
            foreach (var obj in lstAll.Where(x => x.IS_CUSTOMER == isCustomer))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetAllVendors(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<Repository.Implement.MD.VendorRepo>().GetAll();
            foreach (var obj in lstAll.Where(x => x != null))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetVendors(Guid projectId, bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<Repository.Implement.PS.ContractRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId)
                .Select(x => x.Vendor)
                .OrderBy(x => x.NAME)
                .Distinct()
                .ToList();
            foreach (var obj in lstAll.Where(x => x != null))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME + " - " + obj.CODE + " - " + obj.MST + " - " + obj.SHORT_NAME });
            }
            return new SelectList(lstData.Distinct(), "Value", "Text", selected);
        }

        public static SelectList GetCustomers(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<CustomerRepo>().GetAllOrdered("NAME");
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME + " - " + obj.CODE + " - " + obj.MST + " - " + obj.SHORT_NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }
        
        public static SelectList GetPsUnits(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<UnitRepo>().GetAllOrdered("NAME");
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.CODE + "-" + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetPsCurrencys(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<CurrencyRepo>().GetAllOrdered("NAME");
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME + " - " + obj.FULL_NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetPsRoles(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectRoleRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.NAME))
            {
                lstData.Add(new Data { Value = obj.ID, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetProjectUserTypes(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectUserTypeRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.NAME))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetUserType(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            lstData.Add(new Data { Value = UserType.Fecon, Text = UserType.GetText(UserType.Fecon) });
            lstData.Add(new Data { Value = UserType.Vendor, Text = UserType.GetText(UserType.Vendor) });

            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetTitles(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<TitleRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.NAME))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetPaymentStatus(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<PaymentStatusRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.NAME))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }
        public static SelectList GetContractStatus(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ContractStatusRepo>().GetAll();
            foreach (var obj in lstAll.OrderBy(x => x.NAME))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }


        public static SelectList GetProjectStructStatus(bool isAddBlank = true, string selected = "")
        {
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }

            lstData.Add(new Data { Value = ProjectStructStatus.KHOI_TAO.GetValue(), Text = ProjectStructStatus.KHOI_TAO.GetName() });
            lstData.Add(new Data { Value = ProjectStructStatus.DANG_THUC_HIEN.GetValue(), Text = ProjectStructStatus.DANG_THUC_HIEN.GetName() });
            lstData.Add(new Data { Value = ProjectStructStatus.TAM_DUNG.GetValue(), Text = ProjectStructStatus.TAM_DUNG.GetName() });
            lstData.Add(new Data { Value = ProjectStructStatus.HOAN_THANH.GetValue(), Text = ProjectStructStatus.HOAN_THANH.GetName() });

            return new SelectList(lstData, "Value", "Text", selected);
        }


        public static SelectList GetTimeTypes(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<TimeTypeRepo>().GetListActive();
            foreach (var obj in lstAll.OrderBy(x => x.NAME))
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }
        
        public static SelectList GetBoqInProject(Guid? projectId, bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId && x.TYPE == ProjectEnum.BOQ.ToString())
                .OrderBy(x => x.C_ORDER)
                .ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.ID.ToString(), Text = $"{obj.GEN_CODE} - {obj.TEXT}" });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static IList GetActivityWbsInProject(Guid? projectId)
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstAll = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => (x.PROJECT_ID == projectId && x.TYPE == ProjectEnum.ACTIVITY.ToString()) ||  (x.TYPE == ProjectEnum.WBS.ToString() && x.PROJECT_ID == projectId) )
                .OrderBy(x => x.C_ORDER)
                .ToList();
            
            return lstAll.ToList();
        }

        
        public static SelectList GetBoqCodeInProject(Guid? projectId, bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectStructRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId && x.TYPE == ProjectEnum.BOQ.ToString())
                .OrderBy(x => x.C_ORDER)
                .ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.ID.ToString(), Text = obj.GEN_CODE });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }
        
        public static SelectList GetProjectResources(Guid projectId, bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectResourceRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId)
                .OrderBy(x => x.USER_NAME)
                .ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.ID.ToString(), Text = $"{obj?.User?.FULL_NAME} ({obj?.USER_NAME})" });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetWorkflowInProject(Guid projectId, bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectWorkflowRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId)
                .ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.ID.ToString(), Text = obj.CODE + " - " + obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetUserInProject(Guid projectId, bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<ProjectResourceRepo>().Queryable()
                .Where(x => x.PROJECT_ID == projectId)
                .OrderBy(x => x.USER_NAME)
                .ToList();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.USER_NAME, Text = $"{obj?.User?.FULL_NAME} ({obj?.USER_NAME})" });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }

        public static SelectList GetListTaskStatuses()
        {
            var lstData = new List<Data>()
            {
                new Data() { Value = "01", Text = "Chưa thực hiện"},
                new Data() { Value = "02", Text = "Đang thực hiện"},
                new Data() { Value = "03", Text = "Dừng thực hiện"},
                new Data() { Value = "04", Text = "Đã hoàn thành"},
            };
            return new SelectList(lstData, "Value", "Text");
        }
        
        public static SelectList GetListTaskPriorities()
        {
            var lstData = new List<Data>()
            {
                new Data() { Value = "01", Text = "Cao"},
                new Data() { Value = "02", Text = "Bình thường"},
                new Data() { Value = "03", Text = "Thấp"},
            };
            return new SelectList(lstData, "Value", "Text");
        }

        public static SelectList GetCurrency(bool isAddBlank = true, string selected = "")
        {
            IUnitOfWork UnitOfWork = new NHUnitOfWork();
            var lstData = new List<Data>();

            if (isAddBlank)
            {
                lstData.Add(new Data() { Value = "", Text = " - " });
            }
            var lstAll = UnitOfWork.Repository<CurrencyRepo>().GetAll();
            foreach (var obj in lstAll)
            {
                lstData.Add(new Data { Value = obj.CODE, Text = obj.NAME });
            }
            return new SelectList(lstData, "Value", "Text", selected);
        }
    }
}
