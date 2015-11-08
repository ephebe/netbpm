using System;
using System.Collections;

namespace NetBpm.Workflow.Delegation
{
	/// <summary> handles the launching and the processing of the results of a sub-process inside an activity.</summary>
	public interface IProcessInvocationHandler
	{
		/// <summary> specifies which transition of the sub-process needs to be taken from the start-state.</summary>
		String GetStartTransitionName(IProcessInvocationContext processInvokerContext);

		/// <summary> fills the fields in the subProcessStartActivityForm when the sub-process-activity is entered.
		/// Before this method is called, the NetBpm-system has started a process instance of the sub-process.
		/// The created process instance is therefor in the start-activity.  The form of the start-activity
		/// has to be filled in this method. After this method is finished, the NetBpm-system will perform the start-activity.
		/// The flow of the parent process instance is blocked until the sub-process is finished.
		/// </summary>
		/// <param name="processInvokerContext">is the object that allows the ProcessInvocationHandler-implementator to communicate with the NetBpm process engine.
		/// </param>
		/// <returns> the fieldValues that will be used to perform the start-activity of the invoked process.
		/// </returns>
		IDictionary GetStartAttributeValues(IProcessInvocationContext processInvokerContext);

		/// <summary> collects the data from the sub-process and feeds it in the parent-process.
		/// After this method is finished, the activity is considered finished and the 
		/// original flow is activated again.
		/// </summary>
		/// <param name="processInvocationContext">is the object that allows the ProcessInvocationHandler-implementator to communicate with the NetBpm process engine.
		/// </param>
		IDictionary CollectResults(IProcessInvocationContext processInvocationContext);

		/// <summary> specifies which transition of the parent-process needs to be taken after
		/// completion of the sub-process.
		/// </summary>
		String GetCompletionTransitionName(IProcessInvocationContext processInvokerContext);
	}
}