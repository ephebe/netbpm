namespace NetBpm.Workflow.Delegation
{
	/// <summary> allows a ProcessInvocationHandler-implementor to interact with and get
	/// information from the execution engine.
	/// </summary>
	public interface IProcessInvocationContext : IHandlerContext
	{
		/// <summary> gets the ActionContext for the invoked process instance;</summary>
		IActionContext GetInvokedProcessContext();
	}
}