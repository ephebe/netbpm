using System;

namespace NetBpm.Workflow.Log
{
	public interface IObjectReference : ILogDetail
	{
		Int64 ReferenceId { get; }
		String ClassName { get; }
		Object GetObject();
	}
}