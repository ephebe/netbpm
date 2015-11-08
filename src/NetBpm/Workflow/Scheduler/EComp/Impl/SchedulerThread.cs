using System;
using System.Threading;
using Castle.Core;
using log4net;
using NetBpm.Util.Client;
using NetBpm.Workflow.Scheduler.EComp;

namespace NetBpm.Workflow.Scheduler.EComp.Impl
{
	public class SchedulerThread : IStartable
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (SchedulerThread));
		private Thread jobThread = null;
		private bool _runing = false;

		public bool Runing
		{
			get { return _runing; }
			set { this._runing = value; }
		}

		public SchedulerThread()
		{
		}

		public void Start()
		{
			Runing=true;
			if (jobThread == null)
			{
				ThreadStart myThreadDelegate = new ThreadStart(Run);
				jobThread = new Thread(myThreadDelegate);
				jobThread.Name = "JobThread";
				jobThread.Start();
			}
		}

		public void Stop()
		{
			Runing=false;
			jobThread=null;
		}

		public void Run()
		{
			ISchedulerSessionLocal scheduler = null;

			try 
			{
				scheduler =(ISchedulerSessionLocal)ServiceLocator.Instance.GetService(typeof (ISchedulerSessionLocal));
				while(Runing)
				{
					//do somting
					log.Debug("SchedulerThread ExecuteJobs");
					scheduler.ExecuteJobs();
					//sleep 5 seconds
					Thread.Sleep(5000);
				}
			}
			finally
			{
				ServiceLocator.Instance.Release(scheduler);
			}
		}
	}
}
