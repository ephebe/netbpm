using NUnit.Framework;
using Castle.Windsor;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Scheduler.EComp;
using NetBpm.Workflow.Definition.EComp;
using NetBpm.Workflow.Execution.EComp;
using NetBpm.Workflow.Log.EComp;
using NetBpm.Test;

namespace NetBpm.Test.Workflow
{
	[TestFixture]
	public class WindsorContainerTest
	{
		private IWindsorContainer container;

		[Test]
		public void ContainerTest()
		{
			container = new WindsorContainer(TestHelper.GetConfigDir()+"WindsorContainerTest.xml");

			IOrganisationService organisationSession = (IOrganisationService) container["OrganisationSession"];
			Assert.IsNotNull(organisationSession);
			organisationSession = (IOrganisationService) container[typeof (IOrganisationService)];
			Assert.IsNotNull(organisationSession);

			ISchedulerSessionLocal schedulerSession = (ISchedulerSessionLocal) container["SchedulerSession"];
			Assert.IsNotNull(schedulerSession);
			schedulerSession = (ISchedulerSessionLocal) container[typeof (ISchedulerSessionLocal)];
			Assert.IsNotNull(schedulerSession);

			IProcessDefinitionService definitionSession = (IProcessDefinitionService) container["DefinitionSession"];
			Assert.IsNotNull(definitionSession);
			definitionSession = (IProcessDefinitionService) container[typeof (IProcessDefinitionService)];
			Assert.IsNotNull(definitionSession);

			IExecutionApplicationService executionSession = (IExecutionApplicationService) container["ExecutionSession"];
			Assert.IsNotNull(executionSession);
			executionSession = (IExecutionApplicationService) container[typeof (IExecutionApplicationService)];
			Assert.IsNotNull(executionSession);

			ILogSessionLocal logSession = (ILogSessionLocal) container["LogSession"];
			Assert.IsNotNull(logSession);
			logSession = (ILogSessionLocal) container[typeof (ILogSessionLocal)];
			Assert.IsNotNull(logSession);
		}

		[Test]
		public void StressTest()
		{
			container = new WindsorContainer(TestHelper.GetConfigDir()+"WindsorContainerTest.xml");
			IProcessDefinitionService definitionSession = null;

			for (int i=1;i<20;i++)
			{
				definitionSession = (IProcessDefinitionService) container["DefinitionSession"];
				Assert.IsNotNull(definitionSession);
				definitionSession = (IProcessDefinitionService) container[typeof (IProcessDefinitionService)];
				Assert.IsNotNull(definitionSession);
				container.Release(definitionSession);
				definitionSession=null;
			}
		}
	}
}
