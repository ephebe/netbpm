using System;

namespace NetBpm.Workflow.Scheduler.EComp
{
	/// <summary> is the remote-interface for the {@link NetBpm.Workflow.Delegation.EComp.SchedulerComponent}</summary>
	public interface ISchedulerSessionLocal
	{
		long ExecuteJobs();
		void ScheduleJob(Job job, String reference);
	}
}