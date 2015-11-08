using System.Collections;
using log4net;
using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Delegation.Impl.Fork
{
	public class AndFork : IForkHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (AndFork));

		public void Fork(IForkContext forkContext)
		{
			log.Debug("starting to fork...");

			IFork fork = (IFork) forkContext.GetNode();
			IEnumerator iter = fork.LeavingTransitions.GetEnumerator();
			while (iter.MoveNext())
			{
				ITransition leavingTransition = (ITransition) iter.Current;

				log.Debug("forking flow for transition " + leavingTransition.Name);
				forkContext.ForkFlow(leavingTransition.Name);
			}
		}
	}
}