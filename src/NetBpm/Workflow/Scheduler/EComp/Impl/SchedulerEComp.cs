using System;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Util.DB;
using NetBpm.Util.EComp;
using NetBpm.Workflow.Organisation.EComp;
using NetBpm.Workflow.Scheduler.Impl;
using Castle.Services.Transaction;
using Castle.Facilities.NHibernateIntegration;

namespace NetBpm.Workflow.Scheduler.EComp.Impl
{
	[Transactional]
	public class SchedulerEComp : NHSessionOpener, ISchedulerSessionLocal
	{
		private static SchedulerComponentImpl implementation = SchedulerComponentImpl.Instance;
		private static readonly ILog log = LogManager.GetLogger(typeof (SchedulerEComp));

		public SchedulerEComp(ISessionManager sessionManager) : base(sessionManager)
		{
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void scheduleJob(Job job)
		{
			ScheduleJob(job, null);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void ScheduleJob(Job job, String reference)
		{
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				implementation.ScheduleTask(job, reference, dbSession);
			}
			catch (Exception e)
			{
				log.Error("error when scheduling job with reference " + reference, e);
			}
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void CancelJobs(String reference)
		{
			DbSession dbSession = null;
			try
			{
				dbSession = OpenSession();
				implementation.CancelTasks(reference, dbSession);
			}
			catch (Exception e)
			{
				log.Error("error when canceling jobs with reference " + reference, e);
			}
		}

		[Transaction(TransactionMode.Requires)]
		public virtual long ExecuteJobs()
		{
			long millisToWait = 0;
			DbSession dbSession = null;
			IOrganisationService organisationComponent = null;
			try
			{
				dbSession = OpenSession();
				organisationComponent = (IOrganisationService) ServiceLocator.Instance.GetService(typeof (IOrganisationService));
				millisToWait = implementation.ExecuteTask(dbSession, organisationComponent);
				ServiceLocator.Instance.Release(organisationComponent);
			}
			catch (Exception e)
			{
				log.Error("error when executing job ", e);
			}
			finally
			{
				ServiceLocator.Instance.Release(organisationComponent);
			}
			return millisToWait;
		}
	}
}