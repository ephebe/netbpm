using System;
using NetBpm.Workflow.Delegation;
using NetBpm.Workflow.Scheduler;
using NetBpm.Workflow.Execution;

namespace NetBpm.Example.Delegate
{
	
	public class ScheduleOvertakingAction : IActionHandler
	{
		public void  Run(IActionContext interactionContext)
		{
			IProcessInstance processInstance = interactionContext.GetProcessInstance();
			
			Job job = new Job(processInstance.ProcessDefinition, "NetBpm.Example.Delegate.OvertakingAction, NetBpm.Example");
			DateTime scheduleDate = DateTime.Now;
//			DateTime scheduleDate = new DateTime((System.DateTime.Now.Ticks - 621355968000000000) / 10000 + 2000);
			job.Date=scheduleDate;
			job.SetAuthentication("cg", "cg");
			
			//When the assigned executor can't be relied on, 
			//director has to step in and make some actions	
			interactionContext.Schedule(job);
		}
	}
}