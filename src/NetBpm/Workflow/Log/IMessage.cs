using System;

namespace NetBpm.Workflow.Log
{
	public interface IMessage : ILogDetail
	{
		//@portme to message after Message->IMassage
		String MessageText { get; }
	}
}