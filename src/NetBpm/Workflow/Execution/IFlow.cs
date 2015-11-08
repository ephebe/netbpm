using System;
using System.Collections;
using Iesi.Collections;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Execution
{
	/// <summary> represents one 'thread-of-execution' for a sequence of {@link Activity}s.
	/// 
	/// <p>Flows hierarchically ordered.  When starting a process instance, a root-flow will be
	/// created. At a fork, the parent-flow will spawn a set of child-flows and then wait until 
	/// they join again.
	/// </p>
	/// 
	/// <p>getNode() represents the state of a flow. 
	/// A flow is always in one of 4 states : 
	/// <ol>
	/// <li>activity : the flow is waiting until an actor performs the activity. All 
	/// state-getters will return null except getNode(), 
	/// which will return an {@link ActivityState}</li> 
	/// <li>fork : a flow is assigned to a fork when it is waiting on its
	/// subflows to join again. All state-getters will return null except getNode(), 
	/// which will return a {@link Fork}</li> 
	/// <li>join : a flow is assigned to a join when it is waiting on its brothers- and 
	/// sister-flows to join before the parent-flow is reactivated All state-getters will 
	/// return null except getNode(), which will return a {@link Join}</li> 
	/// <li>ended : meaning is obvious.
	/// All state-getters will return null.</li> 
	/// </ol>
	/// </p> 
	/// </summary>
	public interface IFlow //: System.Runtime.Serialization.ISerializable
	{
		/// <summary> the meaningless primary-key for this object. </summary>
		Int64 Id { get; }

		IList GetUserMessages();

		/// <summary> is the name of the flow.</summary>
		String Name { get; }

		/// <summary> is a pointer to the {@link Node} in the activity diagram that represents the state of this flow.</summary>
		INode Node { get; }

		/// <summary> are the logged events related to this flow.</summary>
		ISet Logs { get; }

		bool EndHasValue { get; }

		bool StartHasValue { get; }

		/// <summary> is the time at which this flow started.</summary>
		DateTime Start { get; }

		/// <summary> is the time at which this flow ended, null if it didn't end yet.</summary>
		DateTime End { get; }

		/// <summary> are the flow-local attributes.</summary>
		ISet AttributeInstances { get; }

		/// <summary> is the parent-Flow of this flow.  Flows are hierarchically ordered
		/// as described in the general-class-description part above.
		/// </summary>
		IFlow Parent { get; }

		/// <summary> is the {@link ProcessInstance} of this flow.  The {@link ProcessInstance} is
		/// associated with the upper-most parent of this flow.  Flows are hierarchically 
		/// ordered as described in the general-class-description part above.
		/// </summary>
		IProcessInstance ProcessInstance { get; }

		/// <summary> is the collection of child-Flows for this Flow. Flows are hierarchically ordered
		/// as described in the general-class-description part above.
		/// </summary>
		/// <returns> a Collection of Flow's 
		/// </returns>
		ISet Children { get; }

		/// <summary> is the sub-process-instance on which this flow is waiting
		/// in a sub-process-activity.
		/// </summary>
		IProcessInstance GetSubProcessInstance();

		/// <summary> tells if this flow is the root-flow of a processInstance.</summary>
		bool IsRootFlow();

		/// <summary> specifies which User or Group is supposed to act upon the Flow.</summary>
		IActor GetActor();

	}
}