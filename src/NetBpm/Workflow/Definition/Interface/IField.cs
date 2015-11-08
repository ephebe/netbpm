using System;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Definition
{
	public interface IField //: System.Runtime.Serialization.ISerializable
	{
		String Name { get; }
		String Description { get; }
		IAttribute Attribute { get; }
		IState State { get; }
		FieldAccess Access { get; }

		IHtmlFormatter GetHtmlFormatter();
	}
}