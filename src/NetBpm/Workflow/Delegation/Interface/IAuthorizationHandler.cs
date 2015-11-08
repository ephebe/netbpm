using System;
using System.Collections;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> verifies if an authenticated user has enough privileges.  An AuthorizationHandler
	/// can be specified on the level of a process definition.  All method-calls to
	/// the admin and execution component that are related to that process definition 
	/// will call the AuthorizationHandler to validate authorization. 
	/// </summary>
	public interface IAuthorizationHandler
	{
		void CheckRemoveProcessInstance(String authenticatedActorId, Int64 processInstanceId);
		void CheckRemoveProcessDefinition(String authenticatedActorId, Int64 processDefinitionId);
		void CheckStartProcessInstance(String authenticatedActorId, Int64 processDefinitionId, IDictionary attributeValues, String transitionName);
		void CheckGetStartForm(String authenticatedActorId, Int64 processDefinitionId);
		void CheckGetActivityForm(String authenticatedActorId, Int64 flowId);
		void CheckPerformActivity(String authenticatedActorId, Int64 flowId, IDictionary attributeValues, String transitionName);
		void CheckDelegateActivity(String authenticatedActorId, Int64 flowId, String delegateActorId);
		void CheckCancelProcessInstance(String authenticatedActorId, Int64 processInstanceId);
		void CheckCancelFlow(String authenticatedActorId, Int64 flowId);
		void CheckGetFlow(String authenticatedActorId, Int64 flowId);
	}
}