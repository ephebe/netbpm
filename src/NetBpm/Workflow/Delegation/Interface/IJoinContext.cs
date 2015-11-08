using System.Collections;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> allows a Joiner-implementor to interact with and get information from
	/// the execution engine.
	/// </summary>
	public interface IJoinContext : IHandlerContext
	{
		/// <summary> gets all active concurrent flows other then the one 
		/// that is actually arriving in the join. 
		/// </summary>
		IList GetOtherActiveConcurrentFlows();
	}
}