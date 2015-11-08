using System;
using System.Collections;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> allows a Forker-implementor to interact with ant get information
	/// from the process engine. 
	/// </summary>
	public interface IForkContext : IHandlerContext
	{
		void ForkFlow(String transitionName);
		void ForkFlow(String transitionName, IDictionary attributeValues);
	}
}