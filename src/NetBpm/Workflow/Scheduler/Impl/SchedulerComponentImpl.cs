using System;
using System.Collections;
using log4net;
using NetBpm.Util.DB;
using NetBpm.Workflow.Delegation.Impl;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Execution.Impl;
using NetBpm.Workflow.Organisation.EComp;

namespace NetBpm.Workflow.Scheduler.Impl
{
	public class SchedulerComponentImpl
	{
		private const long DEFAULT_INTERVAL = 5000;
		private static readonly DelegationHelper delegationHelper;
		private static readonly ILog log = LogManager.GetLogger(typeof (SchedulerComponentImpl));
		private static readonly SchedulerComponentImpl instance = new SchedulerComponentImpl();

		static SchedulerComponentImpl()
		{
			delegationHelper = DelegationHelper.Instance;
		}

		/// <summary> gets the singleton instance.</summary>
		public static SchedulerComponentImpl Instance
		{
			get { return instance; }

		}

		private SchedulerComponentImpl()
		{
		}

		public void ScheduleTask(Job job, String reference, DbSession dbSession)
		{
			JobImpl jobImpl = new JobImpl(job, reference);
			dbSession.Save(jobImpl);
			dbSession.Flush();
		}

		private const String queryFindByReference = "from a in class NetBpm.Workflow.Scheduler.Impl.JobImpl " +
			"where a.Reference = ? ";

		public void CancelTasks(String reference, DbSession dbSession)
		{
			dbSession.Delete(queryFindByReference, reference, DbType.STRING);
			dbSession.Flush();
		}


		private const String queryFindJobsToBeExecuted = "from a in class NetBpm.Workflow.Scheduler.Impl.JobImpl " +
			"where a.Date <= ? " + "order by a.Date";

		private const String queryFindJobsInTheFuture = "from a in class NetBpm.Workflow.Scheduler.Impl.JobImpl " +
			"where a.Date > ? " + "order by a.Date";

		public long ExecuteTask(DbSession dbSession, IOrganisationService organisationComponent)
		{
			long millisToWait = DEFAULT_INTERVAL;

			DateTime now = DateTime.Now;

			IEnumerator iter = dbSession.Iterate(queryFindJobsToBeExecuted, now, DbType.TIMESTAMP).GetEnumerator();
			if (iter.MoveNext())
			{
				JobImpl job = (JobImpl) iter.Current;

				try
				{
					log.Debug("executing activation '" + job.Id + "' scheduled for " + job.Date.ToString());
					log.Debug("activation's flow-context is :" + job.Context);

					String userId = job.UserId;

					DelegationImpl actionDelegation = job.ActionDelegation;

					ExecutionContextImpl executionContext = new ExecutionContextImpl(userId, dbSession, organisationComponent);
					IFlow context = job.Context;
					if (context != null)
					{
						executionContext.SetFlow(context);
						executionContext.SetProcessInstance(context.ProcessInstance);
						executionContext.SetProcessDefinition(context.ProcessInstance.ProcessDefinition);
					}
					else
					{
						executionContext.SetProcessDefinition(job.ProcessDefinition);
					}

					delegationHelper.DelegateScheduledAction(actionDelegation, executionContext);

				}
				catch (Exception t)
				{
					log.Error("scheduler-exception : couldn't perform task : " + t.Message, t);
				}

				dbSession.Delete(job);
				dbSession.Flush();
				if (iter.MoveNext())
				{
					return 0;
				}
			} 

			iter = dbSession.Iterate(queryFindJobsInTheFuture, now, DbType.TIMESTAMP).GetEnumerator();
			if (iter.MoveNext())
			{
				JobImpl activation = (JobImpl) iter.Current;
				long activationDate = activation.Date.Ticks;
				millisToWait = activationDate - now.Ticks;
				log.Debug("next activation is scheduled at " + activation.Date.ToString() + ", (in " + millisToWait + " millis)");
				if (millisToWait < 0)
					millisToWait = 0;
				if (millisToWait > DEFAULT_INTERVAL)
					millisToWait = DEFAULT_INTERVAL;
			}

			return millisToWait;
		}
	}
}