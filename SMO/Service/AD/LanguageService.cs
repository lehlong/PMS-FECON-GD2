using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SMO.Service.AD
{
    public class LanguageService : GenericService<T_AD_LANGUAGE, LanguageRepo>
    {
        public string LangSource { get; set; }
        public string LangDestination { get; set; }
        public LanguageService() : base()
        {

        }

        public void DongBo()
        {
            try
            {
                var lstSource = this.CurrentRepository.Queryable().Where(x => x.LANG == this.LangSource).ToList();
                var lstNewLang = new List<T_AD_LANGUAGE>();
                UnitOfWork.BeginTransaction();
                foreach (var item in lstSource)
                {
                    if (item.OBJECT_TYPE == "M")
                    {
                        if (!this.CurrentRepository.CheckExist(x => x.FK_CODE == item.FK_CODE && x.OBJECT_TYPE == item.OBJECT_TYPE && x.LANG == LangDestination))
                        {
                            var newLang = new T_AD_LANGUAGE()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                FK_CODE = item.FK_CODE,
                                OBJECT_TYPE = item.OBJECT_TYPE,
                                LANG = LangDestination,
                                VALUE = item.VALUE
                            };
                            if (ProfileUtilities.User != null)
                            {
                                newLang.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                newLang.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                            }
                            this.CurrentRepository.Create(newLang);
                            lstNewLang.Add(newLang);
                        }
                    }
                    else
                    {
                        if (!this.CurrentRepository.CheckExist(x => x.FK_CODE == item.FK_CODE && x.OBJECT_TYPE == item.OBJECT_TYPE && x.FORM_CODE == item.FORM_CODE && x.LANG == LangDestination))
                        {
                            var newLang = new T_AD_LANGUAGE()
                            {
                                PKID = Guid.NewGuid().ToString(),
                                FK_CODE = item.FK_CODE,
                                FORM_CODE = item.FORM_CODE,
                                OBJECT_TYPE = item.OBJECT_TYPE,
                                LANG = LangDestination,
                                VALUE = item.VALUE
                            };
                            if (ProfileUtilities.User != null)
                            {
                                newLang.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                newLang.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                            }
                            this.CurrentRepository.Create(newLang);
                            lstNewLang.Add(newLang);
                        }
                    }
                }
                UnitOfWork.Commit();

                foreach (var item in lstNewLang)
                {
                    LanguageUtilities.AddToCache(new LanguageObject()
                    {
                        Code = item.OBJECT_TYPE + "-" + item.FORM_CODE + "-" + item.FK_CODE,
                        Language = item.LANG,
                        Value = item.VALUE
                    });
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void Update(string value, string id)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                this.Get(id);
                this.ObjDetail.VALUE = value.Trim();
                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    this.ObjDetail.UPDATE_DATE = this.CurrentRepository.GetDateDatabase();
                }
                this.CurrentRepository.Update(this.ObjDetail);
                UnitOfWork.Commit();

                LanguageUtilities.AddToCache(new LanguageObject()
                {
                    Code = this.ObjDetail.OBJECT_TYPE + "-" + this.ObjDetail.FORM_CODE + "-" + this.ObjDetail.FK_CODE,
                    Language = this.ObjDetail.LANG,
                    Value = this.ObjDetail.VALUE
                });
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
