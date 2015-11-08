using System;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Execution
{
	/// <summary> is one execution of a {@link ProcessDefinition}.</summary>
	public interface IProcessInstance //: System.Runtime.Serialization.ISerializable
	{
		/// <summary> the meaningless primary-key for this object. </summary>
		Int64 Id { get; }

		bool EndHasValue { get; }

		bool StartHasValue { get; }

		/// <summary> the date this instance started. </summary>
		DateTime Start { get; }

		/// <summary> the date this instance ended or null if this instance has
		/// not yet ended.
		/// </summary>
		DateTime End { get; }

		/// <summary> is the actor that started this process instance. </summary>
		/// <returns> Actor
		/// </returns>
		IActor GetInitiator();

		/// <summary> the {@link ProcessDefinition} of this instance.</summary>
		IProcessDefinition ProcessDefinition { get; }

		/// <summary> the top-level flow of this instance.  Note that 
		/// forks and joins create a hierarchically ordered
		/// tree of flows.
		/// </summary>
		IFlow RootFlow { get; }

		/// <summary> if this instance is executed as an activity
		/// of a higher-level process, this is the {@link Flow}
		/// which is waiting in a activity for this instance to complete.
		/// </summary>
		/// <seealso cref="NetBpm.Workflow.Delegation.IProcessInvocationHandler">
		/// </seealso>
		IFlow SuperProcessFlow { get; }
	}
}