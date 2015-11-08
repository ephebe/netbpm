using System;
using System.Threading;
using NetBpm.Util.Client;
using NetBpm.Workflow.Scheduler.EComp;

namespace NetBpm.Workflow.Scheduler.Impl
{
	public class SchedulerMain
	{
		private static ISchedulerSessionLocal SchedulerSessionRemote
		{
			get
			{
				ServiceLocator serviceLocator = ServiceLocator.Instance;
				ISchedulerSessionLocal schedulerSessionRemote = (ISchedulerSessionLocal) serviceLocator.GetService(typeof (ISchedulerSessionLocal));
				return schedulerSessionRemote;
			}

		}

		private static long DEFAULT_INTERVAL = 5000;

		[STAThread]
		public static void Main(String[] args)
		{
			try
			{
				bool loop = true;
				if (args.Length != 1)
				{
					Console.Out.WriteLine("SchedulerMain loop(true|false)");
				}
				ServiceLocator sl = ServiceLocator.Instance;

				if (args.Length > 0 && args[0].EndsWith("false"))
				{
					loop = false;
				}
				ISchedulerSessionLocal schedulerSessionRemote = SchedulerSessionRemote;

				while (loop)
				{
					long millisToWait = DEFAULT_INTERVAL;

					try
					{
						if (schedulerSessionRemote != null)
						{
							Console.Out.WriteLine(DateTime.Now + " : scheduling task...");
							millisToWait = schedulerSessionRemote.ExecuteJobs();
						}
						else
						{
							schedulerSessionRemote = SchedulerSessionRemote;
						}
					}
					catch (Exception t)
					{
						Console.WriteLine("Error: " + t.Message);
					}

					Console.Out.WriteLine("going to sleep for " + millisToWait + " milliseconds...");
					Thread.Sleep(new TimeSpan(10000*millisToWait));
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error: " + e.Message);
			}
		}
	}
}