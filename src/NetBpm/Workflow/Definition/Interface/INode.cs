using Iesi.Collections;

namespace NetBpm.Workflow.Definition
{
	public interface INode : IDefinitionObject
	{
		IProcessBlock ProcessBlock { get; }
		ISet ArrivingTransitions { get; }
		ISet LeavingTransitions { get; }
	}
}