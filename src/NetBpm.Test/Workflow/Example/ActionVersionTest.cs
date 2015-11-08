using System.IO;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.EComp;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Example
{
	[TestFixture]
	public class ActionVersionTest
	{
		protected internal ServiceLocator servicelocator = null;
		protected internal IProcessDefinitionService definitionComponent = null;
		protected internal IExecutionApplicationService executionComponent = null;
		protected internal TestUtil testUtil = null;
		private NetBpmContainer _container = null;

		[SetUp]
		public void SetUp()
		{
			SetContainer();
		}

		[TearDown]
		public void TearDown()
		{
			DisposeContainer();
		}
		public void SetContainer()
		{
			//configure the container
			_container = new NetBpmContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"app_config.xml"));
			testUtil = new TestUtil();
			servicelocator = ServiceLocator.Instance;
			definitionComponent = servicelocator.GetService(typeof (IProcessDefinitionService)) as IProcessDefinitionService;
			executionComponent = servicelocator.GetService(typeof (IExecutionApplicationService)) as IExecutionApplicationService;
			testUtil.LoginUser("ae");

		}

		public void DisposeContainer()
		{	
			servicelocator.Release(definitionComponent);
			definitionComponent=null;
			servicelocator.Release(executionComponent);
			executionComponent=null;

			_container.Dispose();
			_container = null;
		}
		
		[Test]
		public void TestActionHandlerVersioning()
		{	
			//This test tests the versioning of Assemblies. 
			//The version 1 Actionhandler always approves and version 2 always disapproves.

			DeployParFile("versiontestV1.par");
			IProcessDefinition definitionV1 = definitionComponent.GetProcessDefinition("ActionHandlerVersionTest");

			// perform the first activity
			IProcessInstance processInstanceV1 = executionComponent.StartProcessInstance(definitionV1.Id);

			Assert.IsNotNull(processInstanceV1);
			Assert.IsTrue(processInstanceV1.RootFlow.Node.Name.Equals("isAvailable"));

			DeployParFile("versiontestV2.par");
			IProcessDefinition definitionV2 = definitionComponent.GetProcessDefinition("ActionHandlerVersionTest");

			// perform the first activity
			IProcessInstance processInstanceV2 = executionComponent.StartProcessInstance(definitionV2.Id);

			Assert.IsNotNull(processInstanceV1);
			Assert.IsTrue(processInstanceV2.RootFlow.Node.Name.Equals("notAvailable"));
		}

		private void DeployParFile(string parFileName)
		{
			FileInfo parFile = new FileInfo(TestHelper.GetExampleDir()+parFileName);
			FileStream fstream = parFile.OpenRead();
			byte[] b = new byte[parFile.Length];
			fstream.Read(b, 0, (int) parFile.Length);
			definitionComponent.DeployProcessArchive(b);
		}
	}
}
