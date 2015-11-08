using System;
using NetBpm.Workflow.Delegation;

namespace NetBpm.Example.Delegate
{

	public class TrivialAssignmentHandler : IAssignmentHandler
	{
		
		public String SelectActor(IAssignmentContext assignerContext)
		{
			String actor = (String) assignerContext.GetConfiguration()["actor"];
			return actor;
		}
	}
}