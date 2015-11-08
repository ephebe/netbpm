using System;
using System.Collections;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Attr;
using NetBpm.Workflow.Execution;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Example
{
    [TestFixture]
	public class HolidayTest : AbstractExampleTest
	{
		protected override String GetParArchiv()
		{
			return "holiday.par";
		}

        [Test]
		public void TestHolidayProcessApproval()
		{
			IDictionary attributeValues = new Hashtable();
			attributeValues["start date"] = DateTime.Now;
			attributeValues["end date"] = new DateTime((DateTime.Now.Ticks - 621355968000000000)/10000 + 9845344);
			attributeValues["comment"] = "going fishing";

			IProcessInstance processInstance = StartNewHolidayRequest("ae", attributeValues);
			Int64 flowId = processInstance.RootFlow.Id;

			attributeValues = new Hashtable();
			attributeValues["evaluation result"] = Evaluation.APPROVE;

			// perform activity-state evaluating
			testUtil.PerformActivity("cg", flowId, 0, attributeValues, executionComponent);

			// perform activity-state HR-notification
			testUtil.PerformActivity("pf", flowId, 1, null, executionComponent);

			// perform activity-state approval notification
			testUtil.PerformActivity("ae", flowId, 1, null, executionComponent);
		}

        [Test]
		public void TestHolidayProcessDisapproval()
		{
			IDictionary attributeValues = new Hashtable();
			attributeValues["start date"] = DateTime.Now;
			attributeValues["end date"] = new DateTime((DateTime.Now.Ticks - 621355968000000000)/10000 + 9845344);
			
			attributeValues["comment"] = "going fishing";

			IProcessInstance processInstance = StartNewHolidayRequest("ae", attributeValues);

			Int64 flowId = processInstance.RootFlow.Id;

			attributeValues = new Hashtable();
			attributeValues["evaluation result"] = Evaluation.DISAPPROVE;

			testUtil.PerformActivity("cg", flowId, 0, attributeValues, executionComponent);

			// perform activity-state disapproval notification
			testUtil.PerformActivity("ae", flowId, 0, null, executionComponent);
		}

        [Test]
		public void TestHolidayDelegation()
		{
			// start the process instance...
			IDictionary attributeValues = new Hashtable();
			attributeValues["start date"] = DateTime.Now;
			attributeValues["end date"] = new DateTime((DateTime.Now.Ticks - 621355968000000000)/10000 + 9845344);
			;
			attributeValues["comment"] = "going fishing";

			IProcessInstance processInstance = StartNewHolidayRequest("ae", attributeValues);

			Int64 flowId = processInstance.RootFlow.Id;

			// perform activity-state evaluating
			attributeValues = new Hashtable();
			attributeValues["evaluation result"] = Evaluation.APPROVE;

			testUtil.PerformActivity("cg", flowId, 0, attributeValues, executionComponent);

			// perform activity-state HR-notification
			testUtil.PerformActivity("pf", flowId, 1, null, executionComponent);

			// delegate the approval notification
			testUtil.DelegateFlow(flowId, 1, "ae", "ed", executionComponent);

			// try to perform someone elses activity
			try
			{
				testUtil.PerformActivity("ae", flowId, 1, null, executionComponent);
				Assert.Fail("an exception was not thrown while performing an activity assigned to someone else");
			}
			catch (SystemException e)
			{
				Assert.IsNotNull(e);
				// ok
			}

			// perform activity-state approval notification
			testUtil.PerformActivity("ed", flowId, 1, null, executionComponent);
		}

        [Test]
		public void TestCancelFirstSubFlow()
		{
			// start the process instance...
			IDictionary attributeValues = new Hashtable();
			attributeValues["start date"] = DateTime.Now;
			attributeValues["end date"] = new DateTime((DateTime.Now.Ticks - 621355968000000000)/10000 + 9845344);
			;
			attributeValues["comment"] = "going fishing";
			IProcessInstance processInstance = StartNewHolidayRequest("ae", attributeValues);

			Int64 flowId = processInstance.RootFlow.Id;

			// perform activity-state evaluating
			attributeValues = new Hashtable();
			attributeValues["evaluation result"] = Evaluation.APPROVE;
			testUtil.PerformActivity("cg", flowId, 0, attributeValues, executionComponent);

			// perform activity-state HR-notification
			testUtil.CancelFlow("pf", flowId, 1, executionComponent);

			// perform activity-state approval notification
			testUtil.PerformActivity("ae", flowId, 1, null, executionComponent);
		}

        [Test]
		public void TestCancelLastSubFlow()
		{
			// start the process instance...
			IDictionary attributeValues = new Hashtable();
			attributeValues["start date"] = DateTime.Now;
			attributeValues["end date"] = new DateTime((DateTime.Now.Ticks - 621355968000000000)/10000 + 9845344);
			;
			attributeValues["comment"] = "going fishing";
			IProcessInstance processInstance = StartNewHolidayRequest("ae", attributeValues);

			Int64 flowId = processInstance.RootFlow.Id;

			// perform activity-state evaluating
			attributeValues = new Hashtable();
			attributeValues["evaluation result"] = Evaluation.APPROVE;
			testUtil.PerformActivity("cg", flowId, 0, attributeValues, executionComponent);

			// perform activity-state approval notification
			testUtil.PerformActivity("ae", flowId, 1, null, executionComponent);

			// perform activity-state HR-notification
			testUtil.CancelFlow("pf", flowId, 1, executionComponent);
		}

        [Test]
		public void TestCancelInstance()
		{
			// start the process instance...
			IDictionary attributeValues = new Hashtable();
			attributeValues["start date"] = DateTime.Now;
			attributeValues["end date"] = new DateTime((DateTime.Now.Ticks - 621355968000000000)/10000 + 9845344);
			;
			attributeValues["comment"] = "going fishing";

			IProcessInstance processInstance = StartNewHolidayRequest("ae", attributeValues);

			Int64 flowId = processInstance.RootFlow.Id;

			// perform activity-state evaluating
			attributeValues = new Hashtable();
			attributeValues["evaluation result"] = Evaluation.APPROVE;
			testUtil.PerformActivity("cg", flowId, 0, attributeValues, executionComponent);

			// perform activity-state HR-notification
			testUtil.CancelInstance("pf", processInstance.Id, executionComponent);
		}

		private IProcessInstance StartNewHolidayRequest(String actorId, IDictionary attributeValues)
		{
			IProcessInstance processInstance = null;
			testUtil.LoginUser(actorId);

			try
			{
				//      loginUtil.login( actorId, actorId );				
				// start the process instance        
				IProcessDefinition holidayRequest = definitionComponent.GetProcessDefinition("Holiday request");

				// perform the first activity
				processInstance = executionComponent.StartProcessInstance(holidayRequest.Id, attributeValues);
				Assert.IsNotNull(processInstance);
			}
			catch (ExecutionException e)
			{
				Assert.Fail("ExcecutionException while starting a new holiday request: " + e.Message);
			}
			finally
			{
				//      loginUtil.logout();
			}

			return processInstance;
		}
	}
}