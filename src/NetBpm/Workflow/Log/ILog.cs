using System;
using System.Collections;
using Iesi.Collections;
using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Organisation;

namespace NetBpm.Workflow.Log
{
	/// <summary> is the log of an event that occured during a process execution, it serves as a 
	/// container for {@link LogDetail}s.
	/// </summary>
	public interface ILog
	{
		Int64 Id { get; }
		IFlow Flow { get; }
		DateTime Date { get; }
		EventType EventType { get; }
		ISet Details { get; }

		IActor GetActor();
		IList GetObjectReferences(String className);
	}
}