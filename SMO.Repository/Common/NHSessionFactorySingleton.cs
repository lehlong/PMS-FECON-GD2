using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Event;
using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SMO.Repository.Common
{
    public static class NHSessionFactorySingleton
    {
        private static ISessionFactory _sessionFactory = null;
        private static object _lockObj = new object();

        public static ISessionFactory SessionFactory
        {
            get
            {
                lock (_lockObj)
                {
                    if (_sessionFactory == null)
                    {
                        try
                        {
                            string strConnection = ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString;
                            FluentConfiguration _configuration = Fluently.Configure()
                                .Database(MsSqlConfiguration.MsSql2008
                                .ConnectionString(strConnection).DoNot.ShowSql())
                                .Mappings(m => CreateMappings(m.FluentMappings))
                                .Cache(c => c
                                .UseQueryCache()
                                .ProviderClass<HashtableCacheProvider>())
                                .ExposeConfiguration(val =>
                                {
                                    val.AppendListeners(ListenerType.PreUpdate, new IPreUpdateEventListener[] { new NHListener() });
                                    val.AppendListeners(ListenerType.PreInsert, new IPreInsertEventListener[] { new NHListener() });
                                });

                            _sessionFactory = _configuration.BuildSessionFactory();
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                return _sessionFactory;
            }
        }

        private static void CreateMappings(FluentMappingsContainer mapping)
        {
            var search = from t in Assembly.GetExecutingAssembly().GetTypes()
                         where t.IsClass && t.Namespace.Contains("SMO.Repository.Mapping.")
                         select t;

            //foreach (var item in search)
            //{
            //    mapping.Add(item);
            //}

            foreach (var item in search)
            {
                if (!item.FullName.Contains("+<>c"))
                {
                    mapping.Add(item);
                }
            }
        }
    }
}
