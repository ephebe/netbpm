using System;
using System.Collections;
using NetBpm.Util.Client;
using NetBpm.Workflow.Organisation.EComp;

namespace NetBpm.Workflow.Execution
{
	/// @todo remove this interface is not in use!!!!!
	/// <summary> is the session facade that exposes the interface for the execution of processes.
	/// <h3>WfMC</h3>
	/// <p>In <a href="http://www.wfmc.org/standards/docs.htm">WfMC-terminology</a> this is interface 2 : Workflow Client API.
	/// </p>
	/// 
	/// <h3 id="authentication">Authentication</h3>
	/// <p>Authentication is letting the ExecutionSession-bean know which user is calling a method.
	/// There are 2 ways of authenticating the User that executes a method upon an ExecutionSession-bean.
	/// <ol>
	/// <li>using J2EE-security and the stateless session bean. See {@link ExecutionSecuredSessionLocalHome}</li>
	/// <li>using the statefull non-secure session bean. See {@link ExecutionSessionLocalHome}.  In this
	/// case, the client can freely specify the user-name.</li>
	/// </ol>
	/// </p>
	/// 
	/// <h3>Component interface</h3>
	/// <p>The two most important interactions with the ExecutionSession-bean are
	/// <ol>
	/// <li><b>Start a process instance</b> : first you need to know the id of the process definition
	/// you want to start.  This can be done by e.g. {@link #getProcessDefinitions()}.  Then start a process 
	/// instance with {@link #startProcessInstance(Long processDefinitionId)}.
	/// </li>
	/// <li><b>Perform an activity for a flow</b> : first a user can get the list of
	/// flows for which he/she is supposed to act through {@link #getTaskList()}.
	/// Then the user can perform the activity with {@link #performActivity(Long,Map)}
	/// </li>
	/// </ol>
	/// </p>
	/// 
	/// </summary>
	/// <seealso cref="IOrganisationService">
	/// </seealso>
	/// <seealso cref="NetBpm.Workflow.Definition.EComp.IProcessDefinitionService">
	/// </seealso>
	public interface ExecutionComponent
	{
		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the authenticated 
		/// user has to perform an activity.
		/// </summary>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">user calling this method</a>.
		/// </returns>
		IList getTaskList();

		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the authenticated 
		/// user has to perform an activity.
		/// </summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Execution.IFlow}s
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">authenticated user</a>.
		/// </returns>
		IList getTaskList(Relations relations);

		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the given
		/// actor has to perform an activity.  Note that an actor can be a group.
		/// </summary>
		/// <param name="actorId">the actor id
		/// </param>
		/// <returns> a Collection of {@link NetBpm.Workflow.Execution.IFlow}s which are 
		/// assigned to the <a href="#authentication">{@link Nbpm.Workflow.Organisation.IActor}
		/// ({@link Nbpm.Workflow.Organisation.IUser} or {@link Nbpm.Workflow.Organisation.IGroup} 
		/// calling this method</a>.
		/// </returns>
		IList getTaskList(String actorId);

		/// <summary> collects all {@link NetBpm.Workflow.Execution.IFlow}s for which the given
		/// actor has to perform an activity.  Note that an actor can be a group.
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
		IList getTaskList(String actorId, Relations relations);

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
		IList getTaskList(IList actorIds);

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
		IList getTaskList(IList actorIds, Relations relations);


		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IProcessInstance startProcessInstance(Int64 processDefinitionId);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IProcessInstance startProcessInstance(Int64 processDefinitionId, IDictionary attributeValues);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IProcessInstance startProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName);

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
		IProcessInstance startProcessInstance(Int64 processDefinitionId, IDictionary attributeValues, String transitionName, Relations relations);

		// @summary@ fetches all web-interface information for the start of a process instance.  
		// @throws AttributeSerializationException if the attributes that are stored in the database as 
		// text can't be parsed to a java-object with the {@link Nbpm.Workflow.Delegation.ISerializer}
		// @throws ExecutionRuntimeException for all other troubles
		// @/summary@
		//@portme web
		//ActivityForm getStartForm(System.Int64 processDefinitionId);
		// @summary@ fetches all web-interface information for the activity of a flow.  
		// @throws AttributeSerializationException if the attributes that are stored in the database as 
		// text can't be parsed to a java-object with the {@link Nbpm.Workflow.Delegation.ISerializer}
		// @throws ExecutionRuntimeException for all other troubles
		// @/summary@
		//@portme web
		//ActivityForm getActivityForm(System.Int64 flowId);
		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IList performActivity(Int64 flowId);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IList performActivity(Int64 flowId, IDictionary attributeValues);

		/// <summary> provides default values (=null) for the optional parameters of 
		/// {@link #startProcessInstance(Long,Map,String,Relations)}
		/// </summary>
		IList performActivity(Int64 flowId, IDictionary attributeValues, String transitionName);

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
		IList performActivity(Int64 flowId, IDictionary attributeValues, String transitionName, Relations relations);

		/// <summary> reassigns the {@link Flow} to the specified {@link Actor}.</summary>
		/// <param name="flowId">is the flow which the authenticated user wants to delegate
		/// </param>
		/// <param name="actorId">is the user or group of the user to which the the flow is assigned.
		/// @throws IllegalArgumentException if the flow or the user do not exist.
		/// </param>
		void delegateActivity(Int64 flowId, String actorId);

		/// <summary> cancels a complete process instance including all flows that are part of this process instance.
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </summary>
		void cancelProcessInstance(Int64 processInstanceId);

		/// <summary> cancels one flow without cancelling the complete process instance.
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </summary>
		void cancelFlow(Int64 flowId);

		/// <summary> collects a flow.
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </summary>
		IFlow getFlow(Int64 flowId);

		/// <summary> collects a flow.</summary>
		/// <param name="relations">specifies which {@link Relations} should be resolved in the 
		/// returned {@link NetBpm.Workflow.Execution.IFlow}
		/// @throws IllegalArgumentException if the flow does not exist.
		/// </param>
		IFlow getFlow(Int64 flowId, Relations relations);

		void saveActivity(Int64 flowId, IDictionary attributeValues);
	}
}