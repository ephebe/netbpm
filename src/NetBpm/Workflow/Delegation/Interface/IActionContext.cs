using System;
using NetBpm.Workflow.Execution;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> allows the Interaction-implementor to interact with and get information from
	/// the process engine.
	/// </summary>
	public interface IActionContext : IHandlerContext
	{
		/// <summary> gets the {@link Flow} in which the {@link Action} is executed.</summary>
		new IFlow GetFlow();

		/// <summary> sets the {@link AttributeInstance} for the attribute with name attributeName to the
		/// value attributeValue.   
		/// </summary>
		void SetAttribute(String attributeName, Object attributeValue);
	}
}