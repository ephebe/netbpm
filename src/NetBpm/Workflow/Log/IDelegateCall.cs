using System;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Workflow.Log
{
	public interface IDelegateCall : ILogDetail
	{
		IDelegation Delegation { get; }
		Type GetInterface();
	}
}