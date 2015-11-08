using Iesi.Collections;

namespace NetBpm.Workflow.Definition
{
	public interface IProcessBlock : IDefinitionObject
	{
		ISet Nodes { get; }
		ISet Attributes { get; }
	}
}