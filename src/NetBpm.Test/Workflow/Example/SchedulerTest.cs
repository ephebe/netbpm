using System;
using System.Threading;
using System.Collections;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Definition;
using NUnit.Framework;

namespace NetBpm.Test.Workflow.Example
{
	[TestFixture]
	public class SchedulerTest : AbstractExampleTest
	{
		protected override String GetParArchiv()
		{
			return "scheduler.par";
		}
		/// <summary>
		/// Test if the Scheduler can be started and stopped correctly
		/// </summary>
		[Test]
		[Ignore("ignoring this test method for now")]
		public virtual void  TestSchedulerThread()
		{
			servicelocator.Scheduler.Start();

			IProcessInstance processInstance = StartNewSchedulerSample1("cg", null);
			System.Int64 flowId = processInstance.RootFlow.Id;
			//sleep 10 seconds to give the scheduler the chance to complete the work
			Thread.Sleep(10000);
			
			//now ae 'do bloody thing'
			testUtil.PerformActivity("ae", flowId, 0, null, executionComponent);
			
			//to next activity "do clean thing".
			testUtil.PerformActivity("cg", flowId, 0, null, executionComponent);
			servicelocator.Scheduler.Stop();
		}

		/// <summary> In this scenario, the employee initially assigned to perform 'bloody
		/// thing' (in) never shows up. So, the job is re-assoigned (by the director)
		/// to a robot (ae)
		/// </summary>
		[Test]
		public virtual void  TestRobotDoesTheJob()
		{
			IProcessInstance processInstance = StartNewSchedulerSample1("cg", null);
			System.Int64 flowId = processInstance.RootFlow.Id;
			
			schedulerComponent.ExecuteJobs();
			
			//now ae 'do bloody thing'
			testUtil.PerformActivity("ae", flowId, 0, null, executionComponent);
			
			//to next activity "do clean thing".
			testUtil.PerformActivity("cg", flowId, 0, null, executionComponent);
		}

		/// <summary> In this scenario, the employee initially assigned to perform 'bloody
		/// thing' (in) shows up late (after the job is re-assoigned (by the director)
		/// to a robot (ae)). However, the employee tries to sneakily perform the job.
		/// It should fail (one can not perform activity that is not assigned to
		/// him/her)
		/// </summary>
		[Test]
		public virtual void  TestEmployeeLate()
		{
			IProcessInstance processInstance = StartNewSchedulerSample1("cg", null);
			System.Int64 flowId = processInstance.RootFlow.Id;			
			
			schedulerComponent.ExecuteJobs();
			
			try
			{
				//in tries to 'do bloody thing', but it's too late, as the activity has
				// been assigned to robot (ae)
				testUtil.PerformActivity("in", flowId, 0, null, executionComponent);
				Assert.Fail("'in' shouldn't be able to perform bloody thing because that activity has been re-assigned to 'ae'");
			}
			catch (System.Exception e)
			{
				Assert.IsNotNull(e.Message);
			}
			
			//now ae 'do bloody thing'
			testUtil.PerformActivity("ae", flowId, 0, null, executionComponent);
			
			//to next activity "do clean thing".
			testUtil.PerformActivity("cg", flowId, 0, null, executionComponent);
		}

		/// <summary> In this scenario, the employee shows up in time (before the schedule of
		/// re-assignment falls).
		/// </summary>
		[Test]
		public virtual void  TestEmployeeDoesTheJob()
		{
			IProcessInstance processInstance = StartNewSchedulerSample1("cg", null);
			System.Int64 flowId = processInstance.RootFlow.Id;
			
			//perform activity "do bloody thing".
			testUtil.PerformActivity("in", flowId, 0, null, executionComponent);
						
			//Scheduled job has lost the race
			schedulerComponent.ExecuteJobs();
			
			//perform activity "do clean thing".
			testUtil.PerformActivity("cg", flowId, 0, null, executionComponent);
		}

		/// <summary> In this scenario, the employee (in) shows up in time, but too lazy to do
		/// the job. He tries to hand over the activity to a robot (ae). This should
		/// fail, as only director is allows to re-assign an activity
		/// </summary>
		[Test]
		[Ignore("ignoring this test method for now")]
		public virtual void  TestEmployeeCheating()
		{
			IProcessInstance processInstance = StartNewSchedulerSample1("cg", null);
			System.Int64 flowId = processInstance.RootFlow.Id;
			
			try
			{
				testUtil.DelegateFlow(flowId, 0, "in", "ae", executionComponent);
				Assert.Fail("Only director is allowed to delegate an activity");
			}
			catch (System.Exception e)
			{
				Assert.IsNotNull(e.Message);
			}
			
			schedulerComponent.ExecuteJobs();
			
			//now ae 'do bloody thing'
			testUtil.PerformActivity("ae", flowId, 0, null, executionComponent);
			
			//to next activity "do clean thing".
			testUtil.PerformActivity("cg", flowId, 0, null, executionComponent);
		}
			

		private IProcessInstance StartNewSchedulerSample1(String actorId, IDictionary attributeValues)
		{
			IProcessInstance processInstance = null;
			
			try
			{
				testUtil.LoginUser(actorId);
//				loginUtil.Login(actorId, actorId);
				
				// start the process instance
				IProcessDefinition schedulerSample1 = definitionComponent.GetProcessDefinition("Scheduling sample 1");
				
				// perform the first activity
				processInstance = executionComponent.StartProcessInstance(schedulerSample1.Id, attributeValues);
				Assert.IsNotNull(processInstance);
			}
			finally
			{
//				loginUtil.logout();
			}
			
			return processInstance;
		}

	}
}
