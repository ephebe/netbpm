using NetBpm.Workflow.Definition.Impl;

namespace NetBpm.Workflow.Execution.Impl
{
	public class ForkedFlow
	{
		private FlowImpl _flow = null;
		private TransitionImpl _transition = null;

		public TransitionImpl Transition
		{
			get { return this._transition; }

		}

		public FlowImpl Flow
		{
			get { return this._flow; }

		}

		public ForkedFlow(TransitionImpl transition, FlowImpl flow)
		{
			this._transition = transition;
			this._flow = flow;
		}
	}
}