using System;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Definition
{
	public interface IAttribute : IDefinitionObject
	{
		IProcessBlock Scope { get; }

		String InitialValue { get; }

		IDelegation SerializerDelegation { get; }
	}
}