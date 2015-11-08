using System;
using NetBpm.Workflow.Definition.Attr;
using NetBpm.Workflow.Delegation;

namespace NetBpm.VersionTest
{
	/// <summary>
	///This test tests the versioning of Assemblies. 
	///The version 1 Actionhandler always approves and version 2 always disapproves.
	/// </summary>
	public class VersionTestAction : IActionHandler
	{

		public void Run(IActionContext actionContext)
		{
			actionContext.SetAttribute("available",Evaluation.APPROVE);
		}
	}
}
