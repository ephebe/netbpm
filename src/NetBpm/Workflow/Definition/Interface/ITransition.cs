namespace NetBpm.Workflow.Definition
{
	public interface ITransition : IDefinitionObject
	{
		INode From { get; }

		INode To { get; }
	}
}