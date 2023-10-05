using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using SMO.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMO.Repository.Common
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T:BaseEntity
    {
        private Type persitentType = typeof(T);
        //private NHUnitOfWork _unitOfWork;

        public GenericRepository(ISession _session)
        {
            NHibernateSession = _session;
        }

        /// <summary>
        /// </summary>
        public ISession NHibernateSession { get; set; }

        public virtual DateTime GetDateDatabase()
        {
            try
            {
                //ORACLE
                //var dateNow = NHibernateSession.CreateSQLQuery("select sysdate from dual").UniqueResult();
                //MSSSQL
                var dateNow = NHibernateSession.CreateSQLQuery("SELECT GETDATE()").UniqueResult();
                if (dateNow != null)
                {
                    return (DateTime)dateNow;
                }
            }
            catch{}
            return DateTime.Now;
        }

        public virtual int ExecuteUpdate(string sql)
        {
            var query = NHibernateSession.CreateSQLQuery(sql);
            int result = query.ExecuteUpdate();
            return result;
        }

        public virtual IQueryable<T> Queryable()
        {
            return NHibernateSession.Query<T>();
        }

        public virtual IList<T> Search(T objFilter, int pageSize, int pageIndex, out int total)
        {
            total = 0;
            return new List<T>();
        }

        /// <summary>
        /// For entities that have assigned ID's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/reference/en/html/mapping.html#mapping-declaration-id-assigned.
        /// </summary>
        public virtual T Create(T obj)
        {
            NHibernateSession.Save(obj);
            return obj;
        }

        /// <summary>
        /// For entities that have assigned ID's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/reference/en/html/mapping.html#mapping-declaration-id-assigned.
        /// </summary>
        public virtual List<T> Create(List<T> lstObj)
        {
            foreach (var obj in lstObj)
            {
                NHibernateSession.Save(obj);
            }
            return lstObj;
        }

        /// <summary>
        /// For entities with automatatically generated IDs, such as identity, SaveOrUpdate may 
        /// be called when saving a new entity.  SaveOrUpdate can also be called to update any 
        /// entity, even if its ID is assigned.
        /// </summary>
        public virtual T CreateOrUpdate(T obj)
        {
            NHibernateSession.SaveOrUpdate(obj);
            return obj;
        }

        /// <summary>
        /// Update entity
        /// </summary>
        public virtual T Update(T obj)
        {
            NHibernateSession.Update(obj);
            return obj;
        }

        public virtual void Delete(T obj)
        {
            NHibernateSession.Delete(obj);
        }

        public virtual void Delete(List<T> lstObj)
        {
            if (lstObj == null || lstObj.Count == 0)
            {
                return;
            }
            foreach (var item in lstObj)
            {
                NHibernateSession.Delete(item);
            }
        }

        public virtual void Delete(object id)
        {
            var obj = this.Get(id);
            if (obj != null)
            {
                this.Delete(obj);
            }
        }

        public virtual void Delete(List<object> listId)
        {
            foreach (var id in listId)
            {
                this.Delete(id);
            }
        }

        public virtual T Load(object id)
        {
            return NHibernateSession.Load<T>(id);
        }

        public virtual T Get(object id, dynamic param = null)
        {
            return NHibernateSession.Get<T>(id);
        }

        public virtual IList<T> GetByHQL(string hql)
        {
            var obj = NHibernateSession.CreateQuery(hql).List<T>();
            return obj;
        }

        public virtual IList<T> GetByProperty(string property, object value)
        {
            StringBuilder hql = new StringBuilder();
            hql.Append(string.Format("FROM {0} a ", persitentType.FullName));
            hql.Append(string.Format("WHERE a.{0} = ?", property));
            var obj = NHibernateSession.CreateQuery(hql.ToString())
                .SetParameter(0, value)
                .List<T>();

            return obj;
        }

        public virtual IList<T> Paging(IQueryable<T> query,int pageSize, int pageIndex, out int total)
        {
            int startIndex = pageSize * (pageIndex - 1);
            var rowCount = query.ToFutureValue(x => x.Count());
            List<T> lstT = query.Skip(startIndex).Take(pageSize).ToFuture().ToList<T>();
            total = rowCount.Value;
            return lstT;
        }

        public virtual IList<T> Paging(IQueryOver<T> query, int pageSize, int pageIndex, out int total)
        {
            int startIndex = pageSize * (pageIndex - 1);
            var rowCount = query.ToRowCountQuery().FutureValue<int>();
            var lstT = query.Take(pageSize).Skip(startIndex).Future().ToList<T>();
            total = rowCount.Value;
            return lstT;
        }

        public virtual IList<T> GetAll()
        {
            var query = NHibernateSession.Query<T>();
            var result = query.ToList();
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<T>();
            }
        }

        public virtual IList<T> GetListActive()
        {
            var query = NHibernateSession.Query<T>();
            var result = query.Where(x => x.ACTIVE == true).ToList();
            if (result != null && result.Count > 0)
            {
                return result;
            }
            else
            {
                return new List<T>();
            }
        }

        public virtual IList<T> Find(IList<string> strs)
        {
            IList<ICriterion> objs = new List<ICriterion>();
            foreach (string s in strs)
            {
                ICriterion cr1 = Expression.Sql(s);
                objs.Add(cr1);
            }
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            foreach (ICriterion rest in objs)
                NHibernateSession.CreateCriteria(persitentType).Add(rest);

            criteria.SetFirstResult(0);
            return criteria.List<T>();
        }

        public virtual void Detach(T obj)
        {
            NHibernateSession.Evict(obj);
        }

        public virtual IList<T> GetAllOrdered(string propertyName, bool ascending = true)
        {
            Order cr1 = new Order(propertyName, ascending);
            IList<T> objsResult = NHibernateSession.CreateCriteria(persitentType).AddOrder(cr1).List<T>();
            return objsResult;
        }
        
        public virtual bool CheckExist(Func<T, bool> predicate)
        {
            var query = NHibernateSession.Query<T>();
            if (query.Count(predicate) > 0)
            {
                return true;
            }
            NHibernateSession.Clear();
            return false;
        }
    }
}
