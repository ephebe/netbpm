using System;
using System.Runtime.Remoting;
using System.Globalization;
using System.Reflection;

using Castle.Windsor;

using NUnit.Framework;

using NetBpm.Test.Bank.EComp;
//using NetBpm.Test.Bank;

//using NHibernate.Cfg;
//using NHibernate.Tool.hbm2ddl;

namespace NetBpm.Test.BaseService
{
	[TestFixture]
	public class RemotingTest
	{
		protected IWindsorContainer serverContainer;
		protected AppDomain serverDomain;
		protected AppDomain clientDomain;

		[SetUp]
		public void SetUp()
		{	
			// init remote server
			serverDomain = AppDomainFactory.Create("server");
			clientDomain = AppDomainFactory.Create("client");

			serverContainer = CreateRemoteContainer(serverDomain, 
				TestHelper.GetConfigDir()+"remotebank_app_config.xml" );

			// init tables
/*			Configuration cfg = (Configuration) serverContainer[ "nhibernate.factory.cfg" ];
			SchemaExport export = new SchemaExport(cfg);
			export.Create(false, true);*/
		}

		[TearDown]
		public void TearDown()
		{
			// remove tables
/*			Configuration cfg = (Configuration) serverContainer[ "nhibernate.factory.cfg" ];
			SchemaExport export = new SchemaExport(cfg);
			export.Drop(false, true);
*/
			// dispose container
			serverContainer.Dispose();
			serverContainer = null;

			AppDomain.Unload(clientDomain);
			AppDomain.Unload(serverDomain);
		}

		[Test]
		[Ignore("ignoring this test method for now")]
		public void CallRemotService()
		{
			IBankA service = (IBankA) 
			Activator.GetObject( typeof(IBankA), "tcp://localhost:2133/BankA" );
			Assert.IsTrue( RemotingServices.IsTransparentProxy( service ) );
			Assert.IsTrue( RemotingServices.IsObjectOutOfAppDomain(service) );

			service.GetBankAccount("test");

		}

		private IWindsorContainer CreateRemoteContainer(AppDomain domain, String configFile)
		{
			ObjectHandle handle = domain.CreateInstance( 
			typeof(WindsorContainer).Assembly.FullName, 
			typeof(WindsorContainer).FullName, false, BindingFlags.Instance|BindingFlags.Public, null, 
			new object[] { configFile }, 
			CultureInfo.InvariantCulture, null, null );

			return (IWindsorContainer) handle.Unwrap();
		}
	}


}
