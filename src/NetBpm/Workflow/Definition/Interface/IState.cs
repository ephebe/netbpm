using System;

namespace NetBpm.Workflow.Definition
{
	public interface IState : INode
	{
		Int32[] ImageCoordinates { get; }
	}
}