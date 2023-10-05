using NHibernate;
using System;
using System.Data;

namespace SMO.Repository.Common
{
    public interface IUnitOfWork : IDisposable
    {
        ISession GetSession();
        IStatelessSession GetStatelessSession();
        void OpenStatelessSession();
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Commit();
        void Flush();
        void Clear();
        void Rollback();
        bool HasOpenTransaction();
        void CloseSession();
        T Repository<T>() where T : class;
    }
}
