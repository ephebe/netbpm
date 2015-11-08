using System;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> is the delegation interface for selecting the {@link NetBpm.Workflow.Organisation.IUser} or 
	/// {@link NetBpm.Workflow.Organisation.IGroup} that will be assigned as actor for an acitivity.
	/// 
	/// <p>After the assignment of an actor to an activity, the actor is the only user that is 
	/// able to act upon the activity. An actor of an activity can do one of the following actions : 
	/// <ul>
	/// <li>perform the activity</li>
	/// <li>delegate the activity to another user</li>
	/// <li>cancel the flow</li>
	/// <li>cancel the process instance</li>
	/// </ul> 
	/// </p>
	/// 
	/// <p>If an activity is assigned to a {@link NetBpm.Workflow.Organisation.IGroup}, the
	/// members of that group are able to pull the activity to their tasklist.  Currently group-members
	/// can just perform tasks of their group without pulling the task explicitly to their
	/// personal tasklist.  This could result in an exception being thrown from the performActivity
	/// method if 2 users try to perform the same activity at the same time.  It would be a better API
	/// design to have members pull tasks to their personal tasklist before allowing them to 
	/// perform the tasks.  That way there can be no conflict so no exception has to be thrown.  But
	/// this pulling mechanism still has to be implemented.
	/// </p>
	/// 
	/// <p>If the returned id is the id of a {@link NetBpm.Workflow.Organisation.IUser}, that 
	/// {@link NetBpm.Workflow.Organisation.IUser} will get this activity in his/her tasklist.
	/// The {@link NetBpm.Workflow.Organisation.IUser} is then assigned to the activity as described above.
	/// </p>
	/// 
	/// <p>If the returned id is the id of {@link NetBpm.Workflow.Organisation.IGroup}, the 
	/// {@link NetBpm.Workflow.Organisation.IGroup} will be assigned to the activity.
	/// Nobody is able to perform an {@link NetBpm.Workflow.Definition.IActivityState} that is 
	/// assigned to a {@link NetBpm.Workflow.Organisation.IGroup} directly.
	/// First, the wanne-be-actor has to pull the {@link NetBpm.Workflow.Definition.IActivityState}
	/// from his {@link NetBpm.Workflow.Organisation.IGroup} in his own
	/// tasklist.  People are only able to pull activities from their own 
	/// {@link NetBpm.Workflow.Organisation.IGroup}(s).
	/// After someone pulls an {@link NetBpm.Workflow.Definition.IActivityState}, the 
	/// {@link NetBpm.Workflow.Definition.IActivityState} is assigned to the 
	/// {@link NetBpm.Workflow.Organisation.IUser} as described above.
	/// </p>
	/// </summary>
	public interface IAssignmentHandler
	{
		/// <summary> performs the assignment of a {@link NetBpm.Workflow.Organisation.IUser} or a 
		/// {@link NetBpm.Workflow.Organisation.IGroup} to an {@link NetBpm.Workflow.Definition.IActivityState}
		/// in a {@link NetBpm.Workflow.Execution.IFlow}.
		/// </summary>
		/// <param name="assignerContext">is the object that allows the Assigner-implementator to communicate with the NetBpm process engine.
		/// </param>
		/// <returns> the id of the actor (= user or group) that must be assigned to the {@link NetBpm.Workflow.Definition.IActivityState}
		/// </returns>
		String SelectActor(IAssignmentContext assignerContext);
	}
}