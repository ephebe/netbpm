using System.Collections;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm.Util.Client;
using NetBpm.Workflow.Definition.EComp;
using NUnit.Framework;

namespace NetBpm.Test.Workflow
{
	[TestFixture]
	public class NetBpmContainerTest
	{
		[Test]
		public void ContainerStressTest()
		{
			for (int i=1;i<5;i++)
			{
				//configure the container
				NetBpmContainer container = new NetBpm.NetBpmContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"app_config.xml"));
				IProcessDefinitionService definitionComponent = null;

				definitionComponent = container["DefinitionSession"] as IProcessDefinitionService;
				IList definitions = definitionComponent.GetProcessDefinitions(null);

				Assert.IsNotNull(definitions);

				ServiceLocator.Instance.Release(definitionComponent);
				container.Dispose();
				container=null;
			}
		}

		[Test]
		public void ServiceLocatorStressTest()
		{
			for (int i=1;i<5;i++)
			{
				//configure the container
				NetBpmContainer container = new NetBpm.NetBpmContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"app_config.xml"));
				IProcessDefinitionService definitionComponent = null;

				definitionComponent = ServiceLocator.Instance.GetService(typeof (IProcessDefinitionService)) as IProcessDefinitionService;
				IList definitions = definitionComponent.GetProcessDefinitions(null);

				Assert.IsNotNull(definitions);

				ServiceLocator.Instance.Release(definitionComponent);
				container.Dispose();
				container=null;
			}
		}
	}
}
