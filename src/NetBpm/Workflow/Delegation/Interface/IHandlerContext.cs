using System;
using System.Collections;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Scheduler;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> super interface for all handler context's.  HandlerContext's provide handler-implementations with a means
	/// of interaction with the process engine.
	/// </summary>
	public interface IHandlerContext
	{
		/// <summary> gets the configuration, specified with the 'parameter'-tags in the process archive.</summary>
		IDictionary GetConfiguration();

		/// <summary> gets the {@link ProcessDefinition}.</summary>
		IProcessDefinition GetProcessDefinition();

		/// <summary> gets the {@link ProcessInstance}.</summary>
		IProcessInstance GetProcessInstance();

		/// <summary> gets the {@link Flow} in which this delegation is executed. </summary>
		IFlow GetFlow();

		/// <summary> gets the {@link Node} in which this delegation is executed.</summary>
		INode GetNode();

		/// <summary> gets the value of an {@link AttributeInstance} associated with this {@link ProcessInstance}.</summary>
		Object GetAttribute(String name);

		/// <summary> allows the Delegate-implementations to log events to the database.</summary>
		void AddLog(String msg);

		// <summary> convenience method for scheduling jobs</summary>
		void  Schedule(Job job);

		// <summary> convenience method for scheduling jobs</summary>
		void  Schedule(Job job, String reference);
	}
}