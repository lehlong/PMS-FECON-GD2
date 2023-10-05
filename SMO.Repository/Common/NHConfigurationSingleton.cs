using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Cache;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SMO.Repository.Common
{
    public static class NHConfigurationSingleton
    {
        private static FluentConfiguration _configuration = null;
        private static object _lockObj = new object();

        public static FluentConfiguration Configuration
        {
            get
            {
                try
                {
                    lock (_lockObj)
                    {
                        if (_configuration == null)
                        {
                            //string strConnection = ConfigurationManager.ConnectionStrings["OracleConnection"].ConnectionString;
                            //_configuration = Fluently.Configure()
                            //    .Database(OracleClientConfiguration.Oracle10
                            //    .ConnectionString(strConnection)
                            //    .Driver<NHibernate.Driver.OracleClientDriver>())
                            //    .Mappings(m => CreateMappings(m.FluentMappings))
                            //    .ExposeConfiguration(config =>
                            //    {
                            //        SchemaExport schemaExport = new SchemaExport(config);
                            //    });


                            string strConnection = ConfigurationManager.ConnectionStrings["SMO_MSSQL_Connection"].ConnectionString;
                            _configuration = Fluently.Configure()
                                .Database(MsSqlConfiguration.MsSql2008
                                .ConnectionString(strConnection))
                                .Mappings(m => CreateMappings(m.FluentMappings))
                                .Cache(c => c
                                .UseQueryCache()
                                .ProviderClass<HashtableCacheProvider>());
                        }
                    }
                }

                catch (Exception ex)
                {
                    throw ex;
                }
                
                return _configuration;
            }
        }

        private static void CreateMappings(FluentMappingsContainer mapping)
        {
            var search = from t in Assembly.GetExecutingAssembly().GetTypes()
                    where t.IsClass && t.Namespace.Contains("SMO.Repository.Mapping.")
                    select t;

            foreach (var item in search)
            {
                mapping.Add(item);
            }
        }
    }
}
