using System;
using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Delegation
{
	public interface IDelegation //: System.Runtime.Serialization.ISerializable
	{
		Int64 Id { get; }
		IProcessDefinition ProcessDefinition { get; }
		String ClassName { get; }
		String Configuration { get; }
		ExceptionHandlingType ExceptionHandlingType { get; }

		Object GetDelegate();
	}
}