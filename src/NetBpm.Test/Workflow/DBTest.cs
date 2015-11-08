using System.Configuration;
using Castle.Facilities.AutoTx;
using Castle.Facilities.NHibernateIntegration;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using Castle.Windsor;
using Configuration = NHibernate.Cfg.Configuration;


namespace NetBpm.Test.Workflow
{
	[TestFixture]
	public class DBTest
	{
		protected IWindsorContainer container;

        [SetUp]
        public void Init()
        {
            container = new WindsorContainer(TestHelper.GetConfigDir() + "WindsorContainerTest.xml");
            //container.AddFacility("transactions", new TransactionFacility());
        }

		[Test]
		public void SimpleGetSession()
		{
			ISessionManager sessionManager = container.Resolve<ISessionManager>();
			Assert.IsNotNull(sessionManager);
			ISession session =  sessionManager.OpenSession();
			Assert.IsNotNull(session);
			//Assert.IsNull(session.Transaction);
			session.Close();
		}

        [Test]
        public void ResetSchema()
        {
            var cfg = this.CreateSQLServer2005(new string[] { "NetBpm","NetBpm.Test" });
            new SchemaExport(cfg).Execute(true, true,false);
        }

        public  Configuration CreateSQLServer2005(string[] lstMappingAssemblyName)
        {
            string connectionString = @"Data Source=HUGO-PC\SQLEXPRESS;Initial Catalog=NetBPM;Integrated Security=SSPI;";
            Configuration configuration = new Configuration()
                .SetProperty(Environment.ReleaseConnections, "on_close")
                .SetProperty(Environment.Dialect, "NHibernate.Dialect.MsSql2005Dialect")
                .SetProperty(Environment.ConnectionDriver, "NHibernate.Driver.SqlClientDriver")
                .SetProperty(Environment.ConnectionString, connectionString)
                .SetProperty(Environment.ShowSql, "true")
                .SetProperty(Environment.ProxyFactoryFactoryClass,
                             "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle")
                .SetProperty(Environment.CacheProvider, "NHibernate.Cache.HashtableCacheProvider");

            foreach (string mappingAssemblyName in lstMappingAssemblyName)
            {
                configuration.AddAssembly(mappingAssemblyName);
            }

            return configuration;
        }

        [TearDown]
        public void Dispose()
        {
            container.Dispose();
            container = null;
        }
	}
}
