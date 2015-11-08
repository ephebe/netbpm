using System;
using System.Collections;
using NetBpm.Util.Client;

namespace NetBpm.Workflow.Execution.EComp
{
	public interface IExecutionApplicationService
	{
		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the authenticated 
		/// user has to perform an activity.
		/// </summary>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">user calling this method</a>.
		/// </returns>
		IList GetTaskList();

		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the authenticated 
		/// user has to perform an activity.
		/// </summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Execution.IFlow}s
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">authenticated user</a>.
		/// </returns>
		IList GetTaskList(Relations relations);

		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the given
		/// actor has to perform an activity.
		/// </summary>
		/// <param name="actorId">the actor id
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">{@link Nbpm.Workflow.Organisation.IActor}
		/// ({@link Nbpm.Workflow.Organisation.IUser} or {@link Nbpm.Workflow.Organisation.IGroup} 
		/// calling this method</a>.
		/// </returns>
		IList GetTaskList(String actorId);

		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the given
		/// actor has to perform an activity.
		/// </summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Execution.IFlow}s
		/// </param>
		/// <param name="actorId">the actor id
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">{@link Nbpm.Workflow.Organisation.IActor}
		/// ({@link Nbpm.Workflow.Organisation.IUser} or {@link Nbpm.Workflow.Organisation.IGroup} 
		/// calling this method</a>.
		/// </returns>
		IList GetTaskList(String actorId, Relations relations);

		/// <summary> collects all{@link NetBpm.Workflow.Execution.IFlow}s for which the given 
		/// collection of actors has to performed an activity.
		/// </summary>
		/// <param name="actorIds">the collection of actor Ids
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">{@link Nbpm.Workflow.Organisation.IActor}
		/// ({@link Nbpm.Workflow.Organisation.IUser} or {@link Nbpm.Workflow.Organisation.IGroup} 
		/// calling this method</a>.
		/// </returns>
		IList GetTaskList(IList actorIds);

		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the given 
		/// collection of actors has to perform an activity.
		/// </summary>
		/// <param name="actorIds">the collection of actor Ids
		/// </param>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the
		/// returned {@link NetBpm.Workflow.Execution.IFlow}s
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">{@link Nbpm.Workflow.Organisation.IActor}
		/// ({@link Nbpm.Workflow.Organisation.IUser} or {@link Nbpm.Workflow.Organisation.IGroup} 
		/// calling this method</a>.
		/// </returns>
		IList GetTaskList(IList actorIds, Relations relations);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IProcessInstance StartProcessInstance(Int64 processDefinitionId);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName);

		/// <summary> creates a new process instance for the given process definition.
		/// Starting a process instance means that the start-activity is performed.
		/// </summary>
		/// <param name="processDefinitionId">the id of a ProcessDefinition
		/// </param>
		/// <param name="attributeValues">maps attribute-names to java-objects.  The java-objects will be stored
		/// in the corresponding attribute-instances.
		/// </param>
		/// <param name="transitionName">specifies which transition should be taken from the start-state in case
		/// there are more then one.
		/// </param>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link Nbpm.Workflow.Execution.IProcessInstance}s
		/// </param>
		/// <returns> the created {@link Nbpm.Workflow.Execution.IProcessInstance}
		/// @throws IllegalArgumentException if processDefinitionId is null or does not correspond with an existing process definition
		/// </returns>
		/// <seealso cref="NetBpm.Workflow.Definition.EComp.IProcessDefinitionService.GetProcessDefinitions()">
		/// use GetProcessDefinitions() to get valid processDefinitionId's
		/// </seealso>
		IProcessInstance StartProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName, Relations relations);

		/// <summary> fetches all web-interface information for the start of a process instance.  
		/// @throws AttributeSerializationException if the attributes that are stored in the database as 
		/// text can't be parsed to a java-object with the {@link Nbpm.Workflow.Delegation.ISerializer}
		/// @throws ExecutionRuntimeException for all other troubles
		/// </summary>
		IActivityForm GetStartForm(Int64 processDefinitionId);

		/// <summary> fetches all web-interface information for the activity of a flow.  
		/// @throws AttributeSerializationException if the attributes that are stored in the database as 
		/// text can't be parsed to a java-object with the {@link Nbpm.Workflow.Delegation.ISerializer}
		/// @throws ExecutionRuntimeException for all other troubles
		/// </summary>
		IActivityForm GetActivityForm(Int64 flowId);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IList PerformActivity(Int64 flowId);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IList PerformActivity(Int64 flowId, IDictionary attributeValues);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IList PerformActivity(Int64 flowId, IDictionary attributeValues, String transitionName);

		/// <summary> performs an {@link ActivityState}.</summary>
		/// <returns> a collection of {@link Flow}'s that are the {@link Flow}s, resulting 
		/// from the {@link Flow} on which the {@link Activity} was done, after processing 
		/// the {@link Activity} input.
		/// </returns>
		/// <param name="flowId">is the id of the flow upon which the client wants to act.
		/// Flows can be obtained by {@link #getTaskList()}
		/// </param>
		/// <param name="attributeValues">maps attribute-names to java-objects.  The java-objects will be stored
		/// in the corresponding attribute-instances.
		/// </param>
		/// <param name="transitionName">specifies which transition should be taken from the start-state in case
		/// there are more then one.
		/// </param>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Execution.IFlow}s
		/// @throws RequiredFieldException if not all required fields are
		/// present in the form-values.
		/// @throws AttributeSerializationException if one of the fieldValues
		/// is not serializable by the {@link Nbpm.Workflow.Delegation.ISerializer} 
		/// specified in the corresponding field.
		/// @throws IllegalArgumentException if the {@link Flow} of the form cannot be found.
		/// @throws IllegalStateException if the {@link Flow} is not in an activity-state.
		/// </param>
		IList PerformActivity(Int64 flowId, IDictionary attributeValues, String transitionName, Relations relations);

		/// <summary> reassigns the {@link Flow} to the specified {@link Actor}.</summary>
		/// <param name="flowId">is the flow which the authenticated user wants to delegate
		/// </param>
		/// <param name="actorId">is the user or group of the user to which the the flow is assigned.
		/// @throws IllegalArgumentException if the flow or the user do not exist.
		/// </param>
		void DelegateActivity(Int64 flowId, String actorId);

		/// <summary> cancels a complete process instance including all flows that are part of this process instance.
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </summary>
		void CancelProcessInstance(Int64 processInstanceId);

		/// <summary> cancels one flow without cancelling the complete process instance.
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </summary>
		void CancelFlow(Int64 flowId);

		/// <summary> collects a flow.
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </summary>
		IFlow GetFlow(Int64 flowId);

		/// <summary> collects a flow.</summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Execution.IFlow}
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </param>
		IFlow GetFlow(Int64 flowId, Relations relations);

		void SaveActivity(Int64 flowId, IDictionary attributeValues);
	}
}