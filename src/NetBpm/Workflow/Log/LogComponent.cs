using System;
using System.Collections;
using NetBpm.Util.Client;
using NetBpm.Workflow.Execution;

namespace NetBpm.Workflow.Log
{
	/// <summary> is the interface for the log component.</summary>
	public interface LogComponent
	{
		IList findProcessInstances(DateTime startedAfter, DateTime startedBefore, String initiatorActorId, String actorId, Int64 processDefinitionId);

		IList findProcessInstances(DateTime startedAfter, DateTime startedBefore, String initiatorActorId, String actorId, Int64 processDefinitionId, Relations relations);

		IProcessInstance getProcessInstance(Int64 processInstanceId);

		/// <summary> gets a processInstance with all its flows recursively.
		/// Note that the relations are resolved on the Flows and not the ProcessInstance.
		/// </summary>
		IProcessInstance getProcessInstance(Int64 processInstanceId, Relations relations);

		IFlow getFlow(Int64 flowId);

		IFlow getFlow(Int64 subFlowId, Relations flowRelations);
	}
}