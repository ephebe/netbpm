using System;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> if more then one {@link NetBpm.Workflow.Definition.ITransition} leaves an 
	/// {@link NetBpm.Workflow.Definition.IActivityState}, a Decision chooses the 
	/// {@link NetBpm.Workflow.Definition.ITransition} 
	/// that has to be activated when the {@link NetBpm.Workflow.Definition.IActivityState}
	/// is performed.
	/// See also <a href="http://www.netbpm.org/docs/architecture.html#The+delegation+principle">The delegation principle</a>.
	/// </summary>
	public interface IDecisionHandler
	{
		/// <summary> decides which of the {@link NetBpm.Workflow.Definition.ITransition}'s, after 
		/// performing an {@link NetBpm.Workflow.Definition.IActivityState}, has to be activated. 
		/// </summary>
		/// <param name="decisionContext">is the object that allows the Decision-implementator to communicate with the NetBpm process engine.
		/// </param>
		/// <returns> the name of the choosen {@link NetBpm.Workflow.Definition.ITransition}.
		/// </returns>
		String Decide(IDecisionContext decisionContext);
	}
}