using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SMO.Repository.Common
{
    public class NHUnitOfWork : IUnitOfWork
    {
        private ISession _session;
        private IStatelessSession _statelessSession;
        private ITransaction _transaction;
        public ISession Session { get { return this._session; } }
        public IStatelessSession StatelessSession { get { return this._statelessSession; } }
        private Dictionary<Type, dynamic> repositories;
       
        public NHUnitOfWork()
        {
            repositories = new Dictionary<Type, dynamic>();
            this.OpenSession();
        }

        public ISession GetSession()
        {
            return Session;
        }

        public IStatelessSession GetStatelessSession()
        {
            return StatelessSession;
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenSession()
        {
            if (this._session == null || !this._session.IsConnected)
            {
                if (this._session != null)
                    this._session.Dispose();

                this._session = NHSessionFactorySingleton.SessionFactory.OpenSession();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void OpenStatelessSession()
        {
            if (this._statelessSession == null || !this._statelessSession.IsConnected)
            {
                if (this._statelessSession != null)
                    this._statelessSession.Dispose();

                this._statelessSession = NHSessionFactorySingleton.SessionFactory.OpenStatelessSession();
            }
        }

        public T Repository<T>() where T : class
        {
            if (repositories.Keys.Contains(typeof(T)) == true)
            {
                return repositories[typeof(T)];//as T
            }
            var repo = Activator.CreateInstance(typeof(T), this);
            repositories.Add(typeof(T), repo);
            return repo as T;
        }

       

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (this._transaction == null || !this._transaction.IsActive)
            {
                if (this._transaction != null)
                    this._transaction.Dispose();

                this._transaction = this._session.BeginTransaction(isolationLevel);
            }
        }

        public void Commit()
        {
            try
            {
                if (HasOpenTransaction())
                {
                    this._transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                //this.Rollback();
                throw ex;
            }
        }

        public void Flush()
        {
            try
            {
                this.Session.Flush();
            }
            catch (Exception ex)
            {
                //this.Rollback();
                throw ex;
            }
        }

        public void Clear()
        {
            try
            {
                this.Session.Clear();
            }
            catch (Exception ex)
            {
                //this.Rollback();
                throw ex;
            }
        }

        public void Rollback()
        {
            try
            {
                if (HasOpenTransaction())
                {
                    this._transaction.Rollback();
                }
            }
            catch
            {
            }
            
            //CloseSession();
        }

        public bool HasOpenTransaction()
        {
            return this._transaction != null && !this._transaction.WasCommitted && !this._transaction.WasRolledBack;
        }

        /// <summary>
        /// Flushes anything left in the session and closes the connection.
        /// </summary>
        public void CloseSession()
        {
            if (this._session != null && this._session.IsOpen)
            {
                //this._session.Flush();
                this._session.Close();
            }
        }

        public void Dispose()
        {
            if (this._transaction != null)
            {
                this._transaction.Dispose();
                this._transaction = null;
            }

            if (this._session != null)
            {
                this._session.Dispose();
                _session = null;
            }
        }
    }
}
