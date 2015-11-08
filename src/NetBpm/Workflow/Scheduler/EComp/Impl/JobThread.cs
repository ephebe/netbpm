using System;
using System.Threading;

namespace NetBpm.Workflow.Scheduler.EComp.Impl
{
	public class JobThread
	{
		private static JobThread jobThreadInstance = new JobThread();
		private bool _runing = true;

		public bool Runing
		{
			get { return _runing; }
			set { this._runing = value; }
		}

		public JobThread()
		{
		}

		public static void StartThread()
		{
			ThreadStart myThreadDelegate = new ThreadStart(jobThreadInstance.Run);
			Thread jobThread = new Thread(myThreadDelegate);
			jobThread.Name = "JobThread";
			jobThread.Start();
		}

		public static void StopThread()
		{
			jobThreadInstance.Runing=false;
		}

		public void Run()
		{
			while(_runing)
			{
				//do somting
				Console.Write("JobThread");
				//sleep one second
				Thread.Sleep(1000);
			}
		}
	}
}
