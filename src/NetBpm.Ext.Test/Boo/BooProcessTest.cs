using System;
using NetBpm.Test.Workflow.Example;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NUnit.Framework;

namespace NetBpm.Boo.Test.Boo
{
	[TestFixture]
	public class BooProcessTest  : AbstractExampleTest
	{
		protected override String GetParArchiv()
		{
			return "booaction.par";
		}

		[Test]
		public void SimpleTest()
		{
			IProcessDefinition booProcess = definitionComponent.GetProcessDefinition("Boo Example");

			// perform the first activity
			IProcessInstance processInstance = executionComponent.StartProcessInstance(booProcess.Id);

			Assert.IsNotNull(processInstance);
			Assert.IsTrue(processInstance.RootFlow.Node.Name.Equals("isAvailable"));
		}
	}
}
