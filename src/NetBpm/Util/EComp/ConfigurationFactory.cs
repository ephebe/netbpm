using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using Configuration = NHibernate.Cfg.Configuration;

namespace NetBpm.Util.EComp
{
    public class ConfigurationFactory
    {
        public static Configuration CreateSQLLite(string sConnectionName, string[] lstMappingAssemblyName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[sConnectionName].ConnectionString;
            Configuration configuration = new Configuration()
                .SetProperty(Environment.ReleaseConnections, "on_close")
                .SetProperty(Environment.Dialect, "NHibernate.Dialect.SQLiteDialect")
                .SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver")
                .SetProperty(Environment.ConnectionString, connectionString)
                .SetProperty(Environment.ShowSql, "true")
                .SetProperty(Environment.ProxyFactoryFactoryClass, "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");

            foreach (string mappingAssemblyName in lstMappingAssemblyName)
            {
                configuration.AddAssembly(mappingAssemblyName);
            }

            return configuration;
        }

        public static Configuration CreateSQLServer2005(string sConnectionName, string[] lstMappingAssemblyName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings[sConnectionName].ConnectionString;
            Configuration configuration = new Configuration()
                .SetProperty(Environment.ReleaseConnections, "on_close")
                .SetProperty(Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect")
                .SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver")
                .SetProperty(Environment.ConnectionString, connectionString)
                .SetProperty(Environment.ShowSql, "true")
                .SetProperty(Environment.ProxyFactoryFactoryClass, "NHibernate.ByteCode.LinFu.ProxyFactoryFactory, NHibernate.ByteCode.LinFu")
                .SetProperty(Environment.CacheProvider, "NHibernate.Cache.HashtableCacheProvider")
                .SetProperty(Environment.UseSecondLevelCache, "true")//這個是用ID去查
                .SetProperty(Environment.UseQueryCache, "true");//這個是用查詢語法，外加指定名稱去查

            foreach (string mappingAssemblyName in lstMappingAssemblyName)
            {
                configuration.AddAssembly(mappingAssemblyName);
            }

            return configuration;
        }
    }
}
