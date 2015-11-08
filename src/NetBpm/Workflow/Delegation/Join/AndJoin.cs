using log4net;

namespace NetBpm.Workflow.Delegation.Impl.Join
{
	public class AndJoin : IJoinHandler
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (AndJoin));

		public bool Join(IJoinContext joinContext)
		{
			bool reactivateParent = false;

			if (joinContext.GetOtherActiveConcurrentFlows().Count == 0)
			{
				reactivateParent = true;
			}

			log.Debug("flow " + joinContext.GetFlow().Name + " is joining : " + reactivateParent);

			return reactivateParent;
		}
	}
}