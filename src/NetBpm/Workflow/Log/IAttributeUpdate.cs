using System;
using NetBpm.Workflow.Definition;

namespace NetBpm.Workflow.Log
{
	public interface IAttributeUpdate : ILogDetail
	{
		IAttribute Attribute { get; }
		String ValueText { get; }
		Object GetValue();
	}
}