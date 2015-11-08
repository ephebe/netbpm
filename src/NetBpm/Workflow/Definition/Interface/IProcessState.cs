using System;

namespace NetBpm.Workflow.Definition
{
	public interface IProcessState : IState
	{
		IProcessDefinition SubProcess { get; }
		String ActorExpression { get; }
	}
}