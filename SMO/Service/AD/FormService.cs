using SMO.Core.Entities;
using SMO.Repository.Implement.AD;
using SMO.Repository.Implement.MD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Service.AD
{
    public class FormService : GenericService<T_AD_FORM, FormRepo>
    {
        public T_AD_FORM_OBJECT ObjObject { get; set; } 
        public List<T_AD_FORM_OBJECT> ObjListObject { get; set; }
        public bool IsCopy { get; set; }
        public string FormCopy { get; set; }
        public FormService() : base()
        {
            ObjObject = new T_AD_FORM_OBJECT();
            ObjListObject = new List<T_AD_FORM_OBJECT>();
            IsCopy = false;
        }

        public void SearchObject()
        {
            try
            {
                int iTotalRecord = 0;
                this.ObjListObject = UnitOfWork.Repository<FormObjectRepo>().Search(this.ObjObject, this.NumerRecordPerPage, this.Page, out iTotalRecord).ToList();
                this.TotalRecord = iTotalRecord;
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
        }

        public void CreateObject()
        {
            try
            {
                if (!UnitOfWork.Repository<FormObjectRepo>().CheckExist(x => x.OBJECT_CODE == this.ObjObject.OBJECT_CODE && x.FK_FORM == this.ObjObject.FK_FORM))
                {
                    UnitOfWork.BeginTransaction();
                    this.ObjObject.PKID = Guid.NewGuid().ToString();
                    UnitOfWork.Repository<FormObjectRepo>().Create(this.ObjObject);

                    var lstLang = UnitOfWork.Repository<DictionaryRepo>().Queryable().Where(x => x.FK_DOMAIN == "LANG" && x.LANG == "vi").ToList();
                    foreach (var item in lstLang)
                    {
                        var obj = new T_AD_LANGUAGE()
                        {
                            PKID = Guid.NewGuid().ToString(),
                            FK_CODE = this.ObjObject.OBJECT_CODE,
                            OBJECT_TYPE = this.ObjObject.TYPE,
                            LANG = item.CODE,
                            FORM_CODE = this.ObjObject.FK_FORM,
                            VALUE = this.ObjObject.OBJECT_CODE
                        };
                        UnitOfWork.Repository<LanguageRepo>().Create(obj);
                    }
                    UnitOfWork.Commit();
                }
                else
                {
                    UnitOfWork.Rollback();
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

        public void UpdateObject()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                UnitOfWork.Repository<FormObjectRepo>().Update(this.ObjObject);
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public override void Create()
        {
            try
            {
                var lstNewLang = new List<T_AD_LANGUAGE>();
                if (!this.CheckExist(x => x.CODE == this.ObjDetail.CODE))
                {
                    UnitOfWork.BeginTransaction();
                    if (ProfileUtilities.User != null)
                    {
                        this.ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                        this.ObjDetail.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                    }

                    this.ObjDetail = this.CurrentRepository.Create(this.ObjDetail);
                    if (this.IsCopy)
                    {
                        //Copy object of form
                        var lstObject = UnitOfWork.Repository<FormObjectRepo>().Queryable().Where(x => x.FK_FORM == this.FormCopy);
                        foreach (var item in lstObject)
                        {
                            UnitOfWork.Repository<FormObjectRepo>().Detach(item);
                            item.PKID = Guid.NewGuid().ToString();
                            item.FK_FORM = this.ObjDetail.CODE;
                            if (ProfileUtilities.User != null)
                            {
                                item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                item.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                            }
                            UnitOfWork.Repository<FormObjectRepo>().Create(item);
                        }

                        //Copy language of object
                        var lstLang = UnitOfWork.Repository<LanguageRepo>().Queryable().Where(x => x.FORM_CODE == this.FormCopy);
                        foreach (var item in lstLang)
                        {
                            UnitOfWork.Repository<LanguageRepo>().Detach(item);
                            item.PKID = Guid.NewGuid().ToString();
                            item.FORM_CODE = this.ObjDetail.CODE;
                            if (ProfileUtilities.User != null)
                            {
                                item.CREATE_BY = ProfileUtilities.User.USER_NAME;
                                item.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                            }
                            lstNewLang.Add(item);
                            UnitOfWork.Repository<LanguageRepo>().Create(item);
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
                else
                {
                    this.State = false;
                    this.MesseageCode = "1101";
                }
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public void GetObjectById(string id)
        {
            this.ObjObject = UnitOfWork.Repository<FormObjectRepo>().Get(id);
        }

        public void DeleteObject(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.BeginTransaction();

                foreach (var item in lstId)
                {
                    var obj = UnitOfWork.Repository<FormObjectRepo>().Get(item);
                    var lstLang = UnitOfWork.Repository<LanguageRepo>().Queryable().Where(x => x.FK_CODE == obj.OBJECT_CODE && x.FORM_CODE == obj.FK_FORM && x.OBJECT_TYPE == obj.TYPE).ToList();
                    UnitOfWork.Repository<LanguageRepo>().Delete(lstLang);
                    UnitOfWork.Repository<FormObjectRepo>().Delete(obj);
                }
                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public override void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                UnitOfWork.BeginTransaction();

                var lstLang = UnitOfWork.Repository<LanguageRepo>().Queryable().Where(x => lstId.Contains(x.FORM_CODE)).ToList();
                UnitOfWork.Repository<LanguageRepo>().Delete(lstLang);
                this.CurrentRepository.Delete(lstId);

                UnitOfWork.Commit();
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
