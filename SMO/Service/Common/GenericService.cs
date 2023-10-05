using SMO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using SMO.Repository.Common;
using NHibernate.Proxy;
using System.Text;

namespace SMO.Service
{
    public class GenericService<T, TRepo> : BaseService, IGenericService<T> where T: BaseEntity where TRepo:GenericRepository<T>
    {
        public T ObjDetail { get; set; }
        public List<T> ObjList { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public IGenericRepository<T> CurrentRepository { get; set; }

        public GenericService() 
        {
            ObjDetail = Activator.CreateInstance(typeof(T)) as T;
            ObjList = new List<T>();
            UnitOfWork = new NHUnitOfWork();
            CurrentRepository = UnitOfWork.Repository<TRepo>();
        }

        public string CreateSMOMaTraCuu() {
            var result = "";
            var randomString = "";
            var isRetry = true;
            while (isRetry)
            {
                randomString = CreateRandomString();
                if (UnitOfWork.GetSession().Query<T_CM_MA_TRA_CUU>().Any(x => x.MA_TRA_CUU == randomString))
                {
                    isRetry = true;
                }
                else
                {
                    try
                    {
                        UnitOfWork.BeginTransaction();
                        UnitOfWork.GetSession().Save(new T_CM_MA_TRA_CUU()
                        {
                            MA_TRA_CUU = randomString
                        });
                        UnitOfWork.Commit();
                        isRetry = false;
                    }
                    catch (Exception)
                    {
                        UnitOfWork.Rollback();
                        isRetry = true;
                    }
                }
            }

            result = DateTime.Now.ToString("yy") + randomString;
            return result;
        }

        private string CreateRandomString()
        {
            const string src = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int length = 5;
            var sb = new StringBuilder();
            Random RNG = new Random();
            for (var i = 0; i < length; i++)
            {
                var c = src[RNG.Next(0, src.Length)];
                sb.Append(c);
            }
            var result = sb.ToString();
            return result;
        }
        public virtual string GetSequence(string modulType)
        {
            var code = ModulType.GetHeaderCode(modulType) + DateTime.Now.AddYears(1).ToString("yy");
            var sequence = UnitOfWork.GetSession().CreateSQLQuery("exec GET_SEQUENCE :ModulType")
                .SetParameter("ModulType", modulType)
                .UniqueResult();
            int iResult = Int32.Parse(sequence.ToString());
            code += string.Format("{0:0000000}", iResult);
            return code;
        }

        public virtual void Search()
        {
            try
            {
                //UnitOfWork.BeginTransaction();

                int iTotalRecord = 0;
                this.ObjList = this.CurrentRepository.Search(this.ObjDetail, this.NumerRecordPerPage, this.Page, out iTotalRecord).ToList();
                this.TotalRecord = iTotalRecord;
                //UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                //UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }


        public virtual void Get(object id, dynamic param = null)
        {
            try
            {
                //UnitOfWork.BeginTransaction();

                this.ObjDetail = this.CurrentRepository.Get(id, param);

                //UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                //UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }


        public virtual void GetAll()
        {
            try
            {
                //UnitOfWork.BeginTransaction();

                this.ObjList = this.CurrentRepository.GetAll().ToList();

                //UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                //UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public virtual void Create()
        {
            try
            {
                UnitOfWork.BeginTransaction();
                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.CREATE_BY = ProfileUtilities.User.USER_NAME;
                    //this.ObjDetail.CREATE_DATE = this.CurrentRepository.GetDateDatabase();
                }

                this.ObjDetail = this.CurrentRepository.Create(this.ObjDetail);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public virtual void Update()
        {
            try
            {
                UnitOfWork.BeginTransaction();

                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    //this.ObjDetail.UPDATE_DATE = this.CurrentRepository.GetDateDatabase();
                }

                this.ObjDetail = this.CurrentRepository.Update(this.ObjDetail);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public virtual void Delete(List<object> lstId)
        {
            try
            {
                UnitOfWork.BeginTransaction();

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

        public virtual void Delete(string strLstSelected)
        {
            try
            {
                var lstId = strLstSelected.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList<object>();
                this.Delete(lstId);
            }
            catch (Exception ex)
            {
                this.State = false;
                this.Exception = ex;
            }
            
        }

        public virtual bool CheckExist(Func<T, bool> predicate)
        {
            try
            {
                //UnitOfWork.BeginTransaction();
                var result = this.CurrentRepository.CheckExist(predicate);
                //UnitOfWork.Commit();
                return result;
            }
            catch
            {
                //UnitOfWork.Rollback();
                return false;
            }
        }

        public virtual void ToggleActive(object id)
        {
            try
            {
                this.Get(id);
                UnitOfWork.BeginTransaction();

                if (ProfileUtilities.User != null)
                {
                    this.ObjDetail.UPDATE_BY = ProfileUtilities.User.USER_NAME;
                    //this.ObjDetail.UPDATE_DATE = this.CurrentRepository.GetDateDatabase();
                }

                this.ObjDetail.ACTIVE = !this.ObjDetail.ACTIVE;
                this.ObjDetail = this.CurrentRepository.Update(this.ObjDetail);

                UnitOfWork.Commit();
            }
            catch (Exception ex)
            {
                UnitOfWork.Rollback();
                this.State = false;
                this.Exception = ex;
            }
        }

        public virtual bool GetState()
        {
            return this.State;
        }
    }
}
