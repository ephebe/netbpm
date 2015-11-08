using System;
using System.Collections;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Log.EComp;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Scheduler.EComp;
using Castle.Windsor.Configuration.Interpreters;
using NUnit.Framework;

namespace NetBpm.Test.Util
{
	[TestFixture]
	public class ServiceLocatorTest
	{
		private ServiceLocator serviceLocator;
		private Object obj = null;
		private NetBpm.NetBpmContainer _container = null;

		/// <summary>Creates a new instance of ServiceLocatorTest </summary>
		public ServiceLocatorTest()
		{
		}


		/* =========== start setUp and tearDown =======================================*/

		[SetUp]
		public void SetUp()
		{
			//configure the container
			_container = new NetBpm.NetBpmContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"app_config.xml"));
			serviceLocator = ServiceLocator.Instance;
			obj = null;
		}

		[TearDown]
		public void TearDown()
		{
			_container.Dispose();
			_container = null;
			serviceLocator = null;
			obj = null;
		}

		/* =========== end setUp and tearDown =========================================*/

		[Test]
		public void StressTest()
		{
			IProcessDefinitionService definitionComponent = null;
			for (int i=1;i<20;i++)
			{
				definitionComponent = ServiceLocator.Instance.GetService(typeof (IProcessDefinitionService)) as IProcessDefinitionService;
				IList definitions = definitionComponent.GetProcessDefinitions(null);
				Assert.IsNotNull(definitions);
				ServiceLocator.Instance.Release(definitionComponent);
			}
		}

		/// <summary> Test get OrganisationComponent Service</summary>
		[Test]
		public void TestGetOrganisationComponentService()
		{
			try
			{
				obj = serviceLocator.GetService(typeof (IOrganisationService));
				Assert.IsNotNull(obj);
//				Assert.IsTrue(obj is OrganisationService);
				serviceLocator.Release(obj);
			}
			catch (SystemException t)
			{
				Assert.Fail("exception should not be thrown when getting organisation component " + t.Message);
			}
		}

		/// <summary> Test get DefinitionComponent Service</summary>
		[Test]
		public void TestGetDefinitionComponentService()
		{
			try
			{
				obj = serviceLocator.GetService(typeof (IProcessDefinitionService));
				Assert.IsNotNull(obj);
//				Assert.IsTrue(obj is ProcessDefinitionService);
				serviceLocator.Release(obj);
			}
			catch (SystemException t)
			{
				Assert.Fail("exception should not be thrown when getting definition component " + t.Message);
			}
		}

		/// <summary> Test get ExecutionComponent Service</summary>
		[Test]
		public void TestGetExecutionComponentService()
		{
			try
			{
				obj = serviceLocator.GetService(typeof (IExecutionApplicationService));
				Assert.IsNotNull(obj);
//				Assert.IsTrue(obj is ExecutionEComp);
				serviceLocator.Release(obj);
			}
			catch (SystemException t)
			{
				Assert.Fail("exception should not be thrown when getting execution component " + t.Message);
			}
		}

		/// <summary> Test get LogComponent Service</summary>
		[Test]
		public void TestGetLogComponentService()
		{
			try
			{
				obj = serviceLocator.GetService(typeof (ILogSessionLocal));
				Assert.IsNotNull(obj);
//				Assert.IsTrue(obj is LogEComp);
				serviceLocator.Release(obj);
			}
			catch (SystemException t)
			{
				Assert.Fail("exception should not be thrown when getting log component. " + t.Message);
			}
		}

		/// <summary> Test get LogComponent Service</summary>
		[Test]
		public void TestGetSchedulerComponentService()
		{
			try
			{
				obj = serviceLocator.GetService(typeof (ISchedulerSessionLocal));
				Assert.IsNotNull(obj);
//				Assert.IsTrue(obj is SchedulerEComp);
				serviceLocator.Release(obj);
			}
			catch (SystemException t)
			{
				Assert.Fail("exception should not be thrown when getting log component. " + t.Message);
			}
		}
	}
}