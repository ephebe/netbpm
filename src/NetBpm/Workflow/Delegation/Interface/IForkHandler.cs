namespace NetBpm.Workflow.Delegation
{
	/// <summary> specifies the behaviour of a {@link NetBpm.Workflow.Definition.IFork}.</summary>
	public interface IForkHandler
	{
		/// <summary> calculates the names of the {@link NetBpm.Workflow.Definition.ITransition}s to be activated
		/// concurrently. The same name can be added multiple times to start multiple instances of the 
		/// same flow.  All of the forked {@link NetBpm.Workflow.Execution.IFlow}s will be synchronized 
		/// in the same {@link NetBpm.Workflow.Definition.IJoin}.  
		/// </summary>
		/// <param name="forkContext">is the object that allows the Forker-implementator to communicate with the netBpm process engine.
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Definition.ITransition}s for which a concurrent {@link NetBpm.Workflow.Execution.IFlow} has to be created.
		/// </returns>
		void Fork(IForkContext forkContext);
	}
}