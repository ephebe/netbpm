using System;
using System.IO;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Scheduler.EComp;
using Castle.Windsor.Configuration.Interpreters;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Example
{

	public abstract class AbstractExampleTest
	{
		protected internal ServiceLocator servicelocator = null;
		protected internal IProcessDefinitionService definitionComponent = null;
		protected internal IExecutionApplicationService executionComponent = null;
		protected internal ISchedulerSessionLocal schedulerComponent = null;
		protected internal IOrganisationService organisationComponent = null;
		protected internal TestUtil testUtil = null;
		private NetBpmContainer _container = null;

//		[TestFixtureSetUp]
		[SetUp]
		public void SetUp()
		{
			SetContainer();
		}

//		[TestFixtureTearDown]
		[TearDown]
		public void TearDown()
		{
			DisposeContainer();
		}

		public void SetContainer()
		{
			//configure the container
			_container = new NetBpmContainer(new XmlInterpreter("app_config.xml"));
			testUtil = new TestUtil();
			servicelocator = ServiceLocator.Instance;
			definitionComponent = servicelocator.GetService(typeof (IProcessDefinitionService)) as IProcessDefinitionService;
			executionComponent = servicelocator.GetService(typeof (IExecutionApplicationService)) as IExecutionApplicationService;
			schedulerComponent = servicelocator.GetService(typeof (ISchedulerSessionLocal)) as ISchedulerSessionLocal;
			organisationComponent = servicelocator.GetService(typeof (IOrganisationService)) as IOrganisationService;
			testUtil.LoginUser("ae");

			// Par是一個壓縮檔，除了有定義檔之外，還有可以用來展出Web-UI定義及相關圖形
			FileInfo parFile = new FileInfo(TestHelper.GetExampleDir()+GetParArchiv());
			FileStream fstream = parFile.OpenRead();
			byte[] b = new byte[parFile.Length];
			fstream.Read(b, 0, (int) parFile.Length);
            //此處在解壓縮Par
			definitionComponent.DeployProcessArchive(b);

		}

		public void DisposeContainer()
		{	
			servicelocator.Release(definitionComponent);
			definitionComponent=null;
			servicelocator.Release(executionComponent);
			executionComponent=null;
			servicelocator.Release(schedulerComponent);
			schedulerComponent=null;
			servicelocator.Release(organisationComponent);
			organisationComponent=null;

			_container.Dispose();
			_container = null;
		}

		protected abstract String GetParArchiv();
	}
}
