using System;

namespace NetBpm.Workflow.Log
{
	public interface IExceptionReport : ILogDetail
	{
		String ExceptionClassName { get; }
		String ExceptionMessage { get; }
		String StackTrace { get; }
	}
}