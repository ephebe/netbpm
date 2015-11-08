namespace NetBpm.Workflow.Delegation
{
	/// <summary> is the delegation-interface for the execution of process-initiated actions.
	/// <p>Interactions are used to communicate with the existing infrastructure in an organisation.
	/// The {@link InteractionContext} allows the Interaction-developer to access all information
	/// about the process instance.
	/// </p>
	/// 
	/// NetBpm provides a lot of ready-to-use, configurable interactions such as...
	/// <ul>
	/// <li>DB-query of update</li>
	/// <li>Sending an email</li>
	/// <li>Calling one or more web-services</li>
	/// <li>Interacting with an ERP-package over JCA</li>
	/// <li>Sending an EDI-message</li>
	/// <li>Putting a message on an enterprise service bus</li>
	/// <li>...</li>
	/// </ul>
	/// </summary>
	public interface IActionHandler
	{
		/// <summary> implements the process-initiated action.</summary>
		/// <param name="actionContext">is the object that allows the Interaction-implementator to communicate with the NetBpm process engine.
		/// </param>
		void Run(IActionContext actionContext); /* added throws Exception, could be remove if found unnecessary */
	}
}