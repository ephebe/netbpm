using System;

namespace NetBpm.Workflow.Execution
{
	/// <summary> is a RuntimeException that is thrown when an actor
	/// tries to perform an operation that is not permitted
	/// by the ExecutionAuthorizationMgr. 
	/// </summary>
	public class AuthorizationException : ExecutionException
	{
		public AuthorizationException(String msg) : base(msg)
		{
		}
	}
}