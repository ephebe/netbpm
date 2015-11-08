using System;

namespace NetBpm.Workflow.Definition
{
	public interface IActivityState : IState
	{
		String ActorRoleName { get; }
	}
}