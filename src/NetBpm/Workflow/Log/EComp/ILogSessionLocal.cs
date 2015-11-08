using System;
using System.Collections;
using NetBpm.Workflow.Execution;

namespace NetBpm.Workflow.Log.EComp
{
	public interface ILogSessionLocal
	{
		IList FindProcessInstances(DateTime startedAfter, DateTime startedBefore, String initiatorActorId, String actorId, Int64 processDefinitionId);

		IProcessInstance GetProcessInstance(Int64 processInstanceId);

		IFlow GetFlow(Int64 flowId);
	}
}