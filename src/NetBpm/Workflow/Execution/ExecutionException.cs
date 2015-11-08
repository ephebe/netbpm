using System;

namespace NetBpm.Workflow.Execution
{
	/// <summary> is a RuntimeException that serves as a base-class for exceptions that are 
	/// related to process execution.
	/// </summary>
	public class ExecutionException : Exception
	{
		public ExecutionException(String msg) : base(msg)
		{
		}
	}
}