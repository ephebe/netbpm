using System;
using System.Collections;
using Castle.Windsor.Configuration.Interpreters;
using NetBpm.Util.Client;
using NetBpm.Workflow.Delegation.Impl.Assignment;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Organisation;
using NetBpm.Workflow.Organisation.EComp;
using NUnit.Framework;

namespace NetBpm.Test.Workflow
{
	[TestFixture]
	public class ActorExpressionTest
	{
		private TestAssignmentContext testAssignmentContext = null;
		private AssignmentExpressionResolver assignmentExpressionResolver = null;

		private NetBpmContainer container;
		private IOrganisationService organisationComponent = null;

		public ActorExpressionTest()
		{
		}

		[SetUp]
		public void SetUp()
		{
			//configure the container
			container = new NetBpm.NetBpmContainer(new XmlInterpreter(TestHelper.GetConfigDir()+"app_config.xml"));
			organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
			testAssignmentContext = new TestAssignmentContext(this,organisationComponent);
			assignmentExpressionResolver = new AssignmentExpressionResolver();
		}

		[TearDown]
		public void TearDown()
		{
			testAssignmentContext = null;
			ServiceLocator.Instance.Release(organisationComponent);
			container.Dispose();
			container = null;
//			serviceLocator.release(organisationComponent);
		}

		[Test]
		public void TestFirstExpressionActor()
		{
			testAssignmentContext.Expression = "actor(ed)";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("ed", actorId);
		}

		[Test]
		public void TestFirstExpressionPreviousActor()
		{
			testAssignmentContext.Expression = "previousActor";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual(testAssignmentContext.GetPreviousActor().Id, actorId);
		}

		[Test]
		public void TestFirstExpressionProcessInitiator()
		{
			testAssignmentContext.Expression = "processInitiator";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual(testAssignmentContext.GetProcessInstance().GetInitiator().Id, actorId);
		}

		[Test]
		public void TestFirstExpressionGroup()
		{
			testAssignmentContext.Expression = "group(group-rd)";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("group-rd", actorId);
		}

		[Test]
		public void TestNextExpressionGroup()
		{
			testAssignmentContext.Expression = "actor(ae)->group(hierarchy)";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("group-rd", actorId);
		}

		[Test]
		public void TestHolidayExpression()
		{
			testAssignmentContext.Expression = "role(boss)";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("cg", actorId);

			testAssignmentContext.Expression = "role(boss)->group(hierarchy)";
			actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("group-rd", actorId);

			testAssignmentContext.Expression = "role(boss)->group(hierarchy)->role(hr-responsible)";
			actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("pf", actorId);
		}

		[Test]
		public void TestNextExpressionRole()
		{
			testAssignmentContext.Expression = "group(group-rd)->role(cfo)";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("ed", actorId);
		}

		[Test]
		public void TestNextExpressionParentGroup()
		{
			testAssignmentContext.Expression = "group(group-rd)->parentGroup";
			String actorId = assignmentExpressionResolver.SelectActor(testAssignmentContext);
			Assert.AreEqual("group-top", actorId);
		}

		private class TestAssignmentContext : ExecutionContextImpl
		{
			private ServiceLocator serviceLocator = ServiceLocator.Instance;
			private static IOrganisationService orgComponent = null;

			private void InitBlock(ActorExpressionTest enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			private ActorExpressionTest enclosingInstance;

			public String Expression
			{
				set
				{
					this.configuration["expression"] = value;
				}

			}

			public ActorExpressionTest Enclosing_Instance
			{
				get { return enclosingInstance; }

			}

			public TestAssignmentContext(ActorExpressionTest enclosingInstance,IOrganisationService organisationComponent)
			{
				orgComponent = organisationComponent;
				InitBlock(enclosingInstance);
				this.previousActor = organisationComponent.FindActorById("ae");
				this.processInstance = new ProcessInstanceImpl();
				this.processInstance.InitiatorActorId = "pf";
				this.configuration = new Hashtable();
				this.attributes = new Hashtable();
				this.attributes["boss"] = organisationComponent.FindActorById("cg");
				this.attributes["requester group"] = organisationComponent.FindGroupById("group-rd");
			}

			public override IOrganisationService GetOrganisationComponent()
			{
				return orgComponent;
			}

/*			public override INode GetNode()
			{
				return null;
			}

			public override IFlow GetFlow()
			{
				return null;
			}
*/
			public override IActor GetPreviousActor()
			{
				return previousActor;
			}

			public override IDictionary GetConfiguration()
			{
				return this.configuration;
			}
/*
			public override IProcessDefinition GetProcessDefinition()
			{
				return null;
			}
*/
			public override IProcessInstance GetProcessInstance()
			{
				return this.processInstance;
			}

			public override Object GetAttribute(String name)
			{
				return attributes[name];
			}
/*
			public override void AddLog(String msg)
			{
			}
*/

/*			public override void  schedule(Job job)
			{
			}
			public override void  schedule(Job job, String reference)
			{
			}
*/
			private IDictionary configuration = null;
			private IDictionary attributes = null;
			private IActor previousActor = null;
			private ProcessInstanceImpl processInstance = null;
		}
	}
}