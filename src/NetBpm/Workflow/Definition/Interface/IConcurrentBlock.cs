namespace NetBpm.Workflow.Definition
{
	public interface IConcurrentBlock : IProcessBlock
	{
		IFork Fork { get; }
		IJoin Join { get; }
	}
}