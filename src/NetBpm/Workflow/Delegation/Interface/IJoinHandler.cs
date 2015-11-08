namespace NetBpm.Workflow.Delegation
{
	/// <summary> allows to specify all kinds of {@link NetBpm.Workflow.Definition.IJoin}-behaviour, 
	/// including run-time process-specific behaviour.
	/// </summary>
	public interface IJoinHandler
	{
		/// <summary> for every {@link NetBpm.Workflow.Execution.IFlow} that arrives in a 
		/// {@link NetBpm.Workflow.Definition.IJoin} a Joiner calculates if the 
		/// parent-{@link NetBpm.Workflow.Execution.IFlow} should be reactivated.
		/// The parent-{@link NetBpm.Workflow.Execution.IFlow} can only be reactivated
		/// once.  The Joiner will not be called for all {@link NetBpm.Workflow.Execution.IFlow}s 
		/// that arrive in a {@link NetBpm.Workflow.Definition.IJoin} after the 
		/// parent-{@link NetBpm.Workflow.Execution.IFlow} is reactivated.
		/// If no Joiner is specified for a Join in the processdefinition.xml, 
		/// the parent-flow will be reactevated when the last active concurrent flow 
		/// arrives in the Join.
		/// </summary>
		/// <param name="joinContext">is the object that allows the Joiner-implementator to communicate with the NetBpm process engine.
		/// </param>
		/// <returns> true if the parent-flow should be reactivated or false otherwise.
		/// </returns>
		bool Join(IJoinContext joinContext);
	}
}